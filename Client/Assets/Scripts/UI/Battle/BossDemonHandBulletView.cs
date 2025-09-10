using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.Battle
{
    [RequireComponent(typeof(UnitBulletConnector))]
    public class BossDemonHandBulletView : BaseView
    {
        [SerializeField] private UnitBulletConnector _connector;
        [SerializeField] private BossDemonHandBulletWaveView _wave;
        [SerializeField] private List<BossDemonHandBulletSegmentView> _segments;
        [SerializeField] private List<BossWeaknessPointView> _weakSpots;
        
        [Header("Base Settings")] 
        public float segmentShowSpeed = 0.1f;
        
        public int Id { get; private set; }
        
        private Action<int, PlayerUnitView> _hitCallback;
        private Transform _target;
        private float _timeout = 10;
        private bool _playerDamaged;
        
        public void Connect(int id, Action<int, PlayerUnitView> hitCallback, Transform target, int ownerUnitId)
        {
            Id = id;
            _hitCallback = hitCallback;
            _target = target;
            _playerDamaged = false;
            for (var i = 0; i < _weakSpots.Count; i++)
            {
                var nextWeakSpot = _weakSpots[i];
                nextWeakSpot.Connect(ownerUnitId);
            }
            StartCoroutine(PlayHand());
        }
        
        private void OnEnable()
        {
            _connector.OnConnect += OnConnect;
            _segments.ForEach(x=>x.Hide());
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
        
        private void OnConnect()
        {
            Connect(_connector.Id, _connector.HitPlayerCallback, _connector.Target, _connector.OwnerUnitId);
        }

        private void OnSegmentHit(PlayerUnitView target)
        {
            if (_playerDamaged)
            {
                return;
            }
            
            _playerDamaged = true;
            _hitCallback?.Invoke(Id, target);
        }

        private void OnWaveHit(PlayerUnitView target)
        {
            if (_playerDamaged)
            {
                return;
            }
            
            _playerDamaged = true;
            _hitCallback?.Invoke(Id, target);
        }
        
        private IEnumerator PlayHand()
        {
            for (var i = 0; i < _segments.Count; i++)
            {
                _segments[i].Connect(OnSegmentHit);
            }

            transform.LookAt(_target);
            var targetPos = _target.position;
            for (var i = 0; i < _segments.Count; i++)
            {
                var nextSegment = _segments[i];
                nextSegment.Show();
                if (Vector3.Distance(nextSegment.transform.position, targetPos) <= 0.6f)
                {
                    nextSegment.transform.localScale = new Vector3(12, nextSegment.transform.localScale.y, 12);
                    var waveView = Instantiate(_wave, null);
                    waveView.Connect(OnWaveHit);
                    var wavePos = targetPos;
                    wavePos.y = 0.1f;
                    waveView.LaunchWave(wavePos);
                    break;
                }
                yield return new WaitForSeconds(segmentShowSpeed);
            }
        }
    }
}