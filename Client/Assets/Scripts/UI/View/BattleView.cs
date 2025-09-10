using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configs;
using UI.BattleStageMainUI;
using UI.FailureWindow;
using UI.VictoryWindow;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using View.Battle;

namespace View
{
    public class BattleView : BaseView
    {
        [SerializeField] private Transform _environmentPlaceholder;
        [SerializeField] private Transform _bulletsPlaceholder;
        
        private BattleStageMainUI _stageMainUI;
        private Action _restartCallback;
        private Action _startBossBattleCallback;
        private Action _quitGameCallback;
        private Action<int, int> _bulletDamageCallback;
        private Action<int, int> _bulletWeaknessSpotDamageCallback;
        private Func<bool> _startRollCallback;
        private Func<bool> _playerMainShotCallback;
        private PlayerUnitView _playerUnitView;
        private Dictionary<int, EnemyUnitView> _enemies = new();
        private BattleEnvironmentView _envView;
        
        public async Task Connect(BattleStageData battleStageData, Action restartCallback, Func<bool> startRollCallback, 
            Func<bool> playerMainShotCallback, Action<int, int> bulletDamageCallback, 
            Action<int, int> bulletWeaknessSpotDamageCallback, Action startBossBattleCallback, Action quitGameCallback)
        {
            _bulletDamageCallback = bulletDamageCallback;
            _bulletWeaknessSpotDamageCallback = bulletWeaknessSpotDamageCallback;
            _startBossBattleCallback = startBossBattleCallback;
            _quitGameCallback = quitGameCallback;
            _startRollCallback = startRollCallback;
            _playerMainShotCallback = playerMainShotCallback;
            _restartCallback = restartCallback;
            _stageMainUI = BattleStageMainUI.Open(new BattleStageMainUIData
            {
            }, true);
            
            for (var i = _environmentPlaceholder.childCount-1; i >= 0; i--)
            {
                var child = _environmentPlaceholder.GetChild(i);
                Destroy(child.gameObject);
            }
            
            var handle = Addressables.InstantiateAsync(battleStageData.EnvironmentView, _environmentPlaceholder);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var envViewObject = handle.Result;
                _envView = envViewObject.GetComponent<BattleEnvironmentView>();
                _envView.Connect(EnterBattleZone);
            }
            
            await Task.Delay(100);
        }
        
        public async Task SpawnPlayer(int unitId, PlayerData playerData)
        {
            var handle = Addressables.InstantiateAsync(playerData.UnitView, _envView.GetPlayerSpawn());
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var playerUnitViewObject = handle.Result;
                _playerUnitView = playerUnitViewObject.GetComponent<PlayerUnitView>();
                _playerUnitView.Connect(unitId, playerData, _startRollCallback, _playerMainShotCallback, 
                    PlayerBulletSpawned, OnHitBullet, OnWeaknessSpotHitBullet);
            }
        }
        
        public async Task SpawnBossUnit(UnitData unitData, int unitId)
        {
            var handle = Addressables.InstantiateAsync(unitData.UnitView, _envView.GetBossSpawn());
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var playerUnitViewObject = handle.Result;
                var enemyUnitView = playerUnitViewObject.GetComponent<EnemyUnitView>();
                enemyUnitView.Connect(unitData, unitId, OnHitBullet);
                _enemies.Add(unitId, enemyUnitView);
            }
        }

        public void SetPlayerMaxHp(float playerMaxHp)
        {
            _stageMainUI.SetPlayerMaxHp(playerMaxHp);
        }

        public void SetPlayerCurrentHp(float playerCurrentHp)
        {
            _stageMainUI.SetPlayerCurrentHp(playerCurrentHp);
        }
        
        public void SetBossCurrentHp(float bossCurrentHp)
        {
            _stageMainUI.SetBossCurrentHp(bossCurrentHp);
        }
        
        public void SetBossMaxHp(float bossMaxHp)
        {
            _stageMainUI.SetBossMaxHp(bossMaxHp);
        }
        
        public void SetBossHpBarActive(bool isHpBarActive)
        {
            _stageMainUI.SetBossHpBarActive(isHpBarActive);
        }
        
        public void SpawnPlayerBullet(int bulletId, string bulletView)
        {
            _playerUnitView.SpawnBullet(bulletId, bulletView);
        }
        
        public void SpawnEnemyBullet(int ownerUnitId, int bulletId, string bulletView)
        {
            if (_enemies.TryGetValue(ownerUnitId, out var unitView))
            {
                unitView.SpawnBullet(bulletId, bulletView, _playerUnitView.PlayerHitPlaceholder);
            }
        }

        public void BossPlayStartPhaseOne(int unitId)
        {
            if (_enemies.TryGetValue(unitId, out var bossUnitView))
            {
                bossUnitView.PlayEnrage(_playerUnitView.transform);
            }
        }
        
        public void PlayUnitAnimation(int unitId, string animationName)
        {
            if (_enemies.TryGetValue(unitId, out var unitView))
            {
                unitView.PlayAnimation(animationName);
            }
        }

        public void ShowComplete()
        {
            VictoryWindow.Open(new VictoryWindowData
            {
                QuitGame = QuitGame,
                Retry = RetryStage
            }, true);
        }

        public void ShowFailure()
        {
            FailureWindow.Open(new FailureWindowData
            {
                QuitGame = QuitGame,
                Retry = RetryStage
            }, true);
        }
        
        private void PlayerBulletSpawned(PlayerGunBulletView bulletView)
        {
            bulletView.transform.SetParent(_bulletsPlaceholder);
        }

        private void OnHitBullet(int bulletId, EnemyUnitView enemyView)
        {
            _bulletDamageCallback?.Invoke(bulletId, enemyView.UnitId);
        }
        
        private void OnHitBullet(int bulletId, PlayerUnitView playerView)
        {
            _bulletDamageCallback?.Invoke(bulletId, playerView.UnitId);
        }

        private void OnWeaknessSpotHitBullet(int bulletId, int enemyUnitId)
        {
            _bulletWeaknessSpotDamageCallback?.Invoke(bulletId, enemyUnitId);
        }

        private void EnterBattleZone()
        {
            _startBossBattleCallback?.Invoke();
        }

        private void QuitGame()
        {
            _quitGameCallback?.Invoke();
        }

        private void RetryStage()
        {
            _restartCallback?.Invoke();
        }
        
        public void Clear()
        {
            if (_stageMainUI != null)
            {
                BattleStageMainUI.Close(_stageMainUI, true);
                _stageMainUI = null;
            }
            for (var i = _environmentPlaceholder.childCount-1; i >= 0; i--)
            {
                var child = _environmentPlaceholder.GetChild(i);
                Destroy(child.gameObject);
            }
            _enemies.Clear();
        }
    }
}