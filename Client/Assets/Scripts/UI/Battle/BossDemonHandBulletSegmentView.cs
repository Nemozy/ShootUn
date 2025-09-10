using System;
using UnityEngine;

namespace View.Battle
{
    public class BossDemonHandBulletSegmentView : BaseView
    {
        private Action<PlayerUnitView> _onHit;
        
        public void Connect(Action<PlayerUnitView> onHit)
        {
            _onHit = onHit;
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
                _onHit?.Invoke(target);
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
                _onHit?.Invoke(target);
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
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}