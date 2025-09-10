using System;
using System.Collections;
using UnityEngine;

namespace View.Battle
{
    [RequireComponent(typeof(UnitBulletConnector))]
    public class BossDemonBeamBulletView : BaseView
    {
        [SerializeField] private UnitBulletConnector _connector;
        [SerializeField] private float _rotationSpeed = 3f;
        
        public int Id { get; private set; }
        
        private Action<int, PlayerUnitView> _hitCallback;
        private Transform _target;
        private Transform _owner;
        private float _timeout = 8;
        private float _timeTick = 0.5f;
        private bool _playerDamagedTick;
        
        public void Connect(int id, Action<int, PlayerUnitView> hitCallback, Transform target, Transform owner)
        {
            Id = id;
            _hitCallback = hitCallback;
            _target = target;
            _owner = owner;
            _playerDamagedTick = true;
            
            StartCoroutine(PlayBeam());
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

            RotateTowardsTarget(_target.position);

            _timeout -= Time.deltaTime;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Obstacle"))
            {
                if (other.TryGetComponent<ObstacleVIew>(out var obstacleWithLogic))
                {
                    obstacleWithLogic.OnDamaged();
                }
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
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                if (collision.gameObject.TryGetComponent<ObstacleVIew>(out var obstacleWithLogic))
                {
                    obstacleWithLogic.OnDamaged();
                }
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
        
        private void RotateTowardsTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
            _owner.rotation = Quaternion.Slerp(
                _owner.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }
        
        private void OnConnect()
        {
            Connect(_connector.Id, _connector.HitPlayerCallback, _connector.Target, _connector.Owner);
        }
        
        private IEnumerator PlayBeam()
        {
            var localScale = transform.localScale;
            var scaleZ = localScale.z;
            var scaleStep = scaleZ / 10;
            localScale = new Vector3(localScale.x, localScale.y, 0);
            transform.localScale = localScale;
            _owner.LookAt(_target);
            transform.LookAt(_target);
            for (var i = 0; i < 10; i++)
            {
                localScale.z += scaleStep;
                transform.localScale = localScale;
                yield return new WaitForSeconds(0.1f);
            }

            localScale.z = scaleZ;
            transform.localScale = localScale;
            
            while (true)
            {
                yield return new WaitForSeconds(_timeTick);
                _playerDamagedTick = false;
            }
        }
    }
}