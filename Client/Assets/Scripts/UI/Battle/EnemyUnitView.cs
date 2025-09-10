using System;
using Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace View.Battle
{
    public class EnemyUnitView : BaseView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _shootPlaceholder;
        
        public int UnitId { get; private set; }
        private Action<int, PlayerUnitView> _hitPlayerBulletCallback;
        
        public void Connect(UnitData unitData, int unitId, Action<int, PlayerUnitView> hitPlayerBulletCallback)
        {
            UnitId = unitId;
            _hitPlayerBulletCallback = hitPlayerBulletCallback;
        }

        public void PlayEnrage(Transform target)
        {
            transform.LookAt(target);
            _animator.Play("Rage");
        }
        
        public void PlayAnimation(string animationName)
        {
            _animator.Play(animationName);
        }
        
        public async void SpawnBullet(int id, string bulletViewAsset, Transform target)
        {
            transform.LookAt(target);
            var handle = Addressables.InstantiateAsync(bulletViewAsset, _shootPlaceholder.position, 
                transform.rotation, null);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var bulletViewObject = handle.Result;
                bulletViewObject.transform.LookAt(target); 
                var bulletConnector = bulletViewObject.GetComponent<UnitBulletConnector>();
                bulletConnector.Connect(id, _hitPlayerBulletCallback, target, this.transform, UnitId);
                //_shootCallback?.Invoke(bulletView);
            }
        }
    }
}