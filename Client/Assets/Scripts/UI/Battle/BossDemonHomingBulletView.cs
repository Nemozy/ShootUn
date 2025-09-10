using System;
using UnityEngine;

namespace View.Battle
{
    [RequireComponent(typeof(UnitBulletConnector))]
    public class BossDemonHomingBulletView : BaseView
    {
        [SerializeField] private UnitBulletConnector _connector;

        [Header("Movement Settings")] 
        public float moveSpeed = 10f;
        public float rotationSpeed = 5f;
        public float maxPredictionTime = 2f;

        [Header("Accuracy Settings")] 
        public float minDistanceToTarget = 0.1f;
        public bool usePrediction = true;

        private Rigidbody rb;
        private Vector3 predictedPosition;

        public int Id { get; private set; }

        private Action<int, PlayerUnitView> _hitCallback;
        private float _timeout = 7;
        private Transform _target;

        public void Connect(int id, Action<int, PlayerUnitView> hitCallback, Transform target)
        {
            Id = id;
            _hitCallback = hitCallback;
            _target = target;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.linearVelocity = transform.forward * moveSpeed;
        }

        private void OnEnable()
        {
            _connector.OnConnect += OnConnect;
        }

        private void OnDisable()
        {
            _connector.OnConnect -= OnConnect;
        }

        private void Update()
        {
            if (_timeout <= 0)
            {
                gameObject.SetActive(false);
                return;
            }

            _timeout -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (_target == null)
            {
                return;
            }

            float distanceToTarget = Vector3.Distance(transform.position, _target.position);
            if (distanceToTarget <= minDistanceToTarget)
            {
                return;
            }

            Vector3 targetPosition = usePrediction ? PredictTargetPosition() : _target.position;
            RotateTowardsTarget(targetPosition);
            MoveTowardsTarget();
        }

        private Vector3 PredictTargetPosition()
        {
            Rigidbody targetRb = _target.GetComponent<Rigidbody>();

            if (targetRb != null && targetRb.linearVelocity.magnitude > 0.1f)
            {
                float predictionTime = CalculatePredictionTime(targetRb);
                predictionTime = Mathf.Min(predictionTime, maxPredictionTime);

                predictedPosition = _target.position + targetRb.linearVelocity * predictionTime;
                return predictedPosition;
            }

            return _target.position;
        }

        private float CalculatePredictionTime(Rigidbody targetRb)
        {
            Vector3 toTarget = _target.position - transform.position;
            float relativeSpeed = (rb.linearVelocity - targetRb.linearVelocity).magnitude;

            if (relativeSpeed < 0.1f) return 0f;

            return toTarget.magnitude / relativeSpeed;
        }

        private void RotateTowardsTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        private void MoveTowardsTarget()
        {
            rb.linearVelocity = transform.forward * moveSpeed;

            // Альтернативный вариант - плавное ускорение к цели
            // Vector3 direction = (target.position - transform.position).normalized;
            // rb.velocity = Vector3.Lerp(rb.velocity, direction * moveSpeed, acceleration * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Obstacle"))
            {
                if (other.TryGetComponent<ObstacleVIew>(out var obstacleWithLogic))
                {
                    obstacleWithLogic.OnDamaged();
                }
                //TODO: убрать в пул
                Destroy(gameObject);
                return;
            }

            if (!other.CompareTag("PlayerUnit"))
            {
                return;
            }

            var target = GetUnitView(other.transform);
            if (target != null)
            {
                _hitCallback?.Invoke(Id, target);
            }

            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                if (collision.gameObject.TryGetComponent<ObstacleVIew>(out var obstacleWithLogic))
                {
                    obstacleWithLogic.OnDamaged();
                }
                //TODO: убрать в пул
                Destroy(gameObject);
                return;
            }

            if (!collision.gameObject.CompareTag("PlayerUnit"))
            {
                return;
            }

            var target = GetUnitView(collision.transform);
            if (target != null)
            {
                _hitCallback?.Invoke(Id, target);
            }

            Destroy(gameObject);
        }

        private PlayerUnitView GetUnitView(Transform go)
        {
            if (go.TryGetComponent<PlayerUnitView>(out var unitView))
            {
                return unitView;
            }

            if (go.parent == null)
            {
                return null;
            }
            

            return GetUnitView(go.parent);
        }

        private void OnConnect()
        {
            Connect(_connector.Id, _connector.HitPlayerCallback, _connector.Target);
        }
    }
}