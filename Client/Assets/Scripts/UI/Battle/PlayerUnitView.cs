using System;
using Configs;
using UnityEngine;

namespace View.Battle
{
    public class PlayerUnitView : BaseView
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Transform _playerHitPlaceholder;

        public int UnitId { get; private set; }
        public Transform PlayerHitPlaceholder => _playerHitPlaceholder;
        
        public void Connect(int unitId, PlayerData playerData, Func<bool> startRollCallback, Func<bool> playerMainShotCallback, 
            Action<PlayerGunBulletView> shootCallback, Action<int, EnemyUnitView> hitBulletCallback,
            Action<int, int> hitBulletWeaknessSpotCallback)
        {
            UnitId = unitId;
            _playerController.Connect(playerData, startRollCallback, playerMainShotCallback, shootCallback,
                hitBulletCallback, hitBulletWeaknessSpotCallback);
        }

        public void SpawnBullet(int id, string bulletView)
        {
            _playerController.SpawnBullet(id, bulletView);
        }
    }
}