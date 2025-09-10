using System;
using UnityEngine;

namespace View.Battle
{
    public class ObstacleVIew : BaseView
    {
        private float _revieveCooldown = 25;
        private float _revieveTimer;
        private Vector3 _startPosition;
        private Vector3 _hidePosition;
        
        private void Start()
        {
            _startPosition = transform.position;
            _hidePosition = _startPosition;
            _hidePosition.y -= 10;
        }

        private void Update()
        {
            if (_revieveTimer > 0)
            {
                _revieveTimer -= Time.deltaTime;
                TryRevive();
            }
        }

        public void OnDamaged()
        {
            _revieveTimer = _revieveCooldown;
            transform.position = _hidePosition;
        }

        private void TryRevive()
        {
            if (_revieveTimer > 0)
            {
                return;
            }

            transform.position = _startPosition;
        }
    }
}