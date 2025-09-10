using System;
using UnityEngine;

namespace View.Battle
{
    public class PlayerGunBulletView : BaseView
    {
        public int Id { get; private set; }
        
        private Action<int, EnemyUnitView> _hitCallback;
        private Action<int, int> _hitWeaknessSpotCallback;
        private float _timeout = 5;
        
        public void Connect(int id, Action<int, EnemyUnitView> hitCallback, Action<int, int> hitWeaknessSpotCallback)
        {
            Id = id;
            _hitCallback = hitCallback;
            _hitWeaknessSpotCallback = hitWeaknessSpotCallback;
        }
        
        private void Update()
        {
            if (_timeout <= 0)
            {
                gameObject.SetActive(false);
                return;
            }

            _timeout -= Time.deltaTime;
            transform.position += transform.forward * 30 * Time.deltaTime;
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
            
            if (!other.CompareTag("Boss"))
            {
                if (other.TryGetComponent<BossWeaknessPointView>(out var weaknessPointView))
                {
                    _hitWeaknessSpotCallback?.Invoke(Id, weaknessPointView.OwnerUnitId);
                }
                if (other.CompareTag("Bullet"))
                {
                    return;
                }
                
                //TODO: убрать в пул
                Destroy(gameObject);
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
            
            if (!collision.gameObject.CompareTag("Boss"))
            {
                if (collision.transform.TryGetComponent<BossWeaknessPointView>(out var weaknessPointView))
                {
                    _hitWeaknessSpotCallback?.Invoke(Id, weaknessPointView.OwnerUnitId);
                }
                if (collision.gameObject.CompareTag("Bullet"))
                {
                    return;
                }
                
                //TODO: убрать в пул
                Destroy(gameObject);
                return;
            }

            var target = GetUnitView(collision.transform);
            if (target != null)
            {
                _hitCallback?.Invoke(Id, target);
            }
            Destroy(gameObject);
        }

        private EnemyUnitView GetUnitView(Transform go)
        {
            if (go.TryGetComponent<EnemyUnitView>(out var unitView))
            {
                return unitView;
            }

            if (go.parent == null)
            {
                return null;
            }

            return GetUnitView(go.parent);
        }
    }
}