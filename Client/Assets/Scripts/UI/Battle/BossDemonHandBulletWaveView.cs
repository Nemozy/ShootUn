using System;
using UnityEngine;
using System.Collections.Generic;

namespace View.Battle
{
    public class BossDemonHandBulletWaveView : BaseView
    {
        [Header("Wave Settings")] 
        public float startRadius = 1f;
        public float maxRadius = 10f;
        public float expansionSpeed = 5f;

        private float currentRadius;
        private bool isExpanding = true;
        private HashSet<GameObject> damagedObjects = new ();

        private Action<PlayerUnitView> _onHit;
        
        public void Connect(Action<PlayerUnitView> onHit)
        {
            _onHit = onHit;
        }
        
        void Start()
        {
            currentRadius = startRadius;
            transform.localScale = new Vector3(startRadius, 1, startRadius);

            UpdateVisualScale();
        }

        void Update()
        {
            if (!isExpanding)
            {
                return;
            }

            currentRadius += expansionSpeed * Time.deltaTime;

            if (currentRadius >= maxRadius)
            {
                isExpanding = false;
                Destroy(gameObject);
                return;
            }

            UpdateVisualScale();
        }

        void UpdateVisualScale()
        {
            transform.localScale = new Vector3(currentRadius, 1, currentRadius);
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
            
            CheckCollisions(other.gameObject);
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

            CheckCollisions(collision.gameObject);
        }

        private void CheckCollisions(GameObject target)
        {
            if (damagedObjects.Contains(target))
            {
                return;
            }

            var distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance <= currentRadius)
            {
                ApplyDamage(target);
                damagedObjects.Add(target);
            }
        }

        private void ApplyDamage(GameObject target)
        {
            _onHit?.Invoke(GetUnitView(target.transform));
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
        
        public void LaunchWave(Vector3 position)
        {
            transform.position = position;

            isExpanding = true;
            currentRadius = startRadius;
            damagedObjects.Clear();
        }
    }
}