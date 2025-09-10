using System;
using System.Threading.Tasks;
using BattleCore;
using Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using View;
using Object = UnityEngine.Object;

namespace Modules
{
    public class BattleModule : ModuleBase
    {
        private GameObject _battleViewObject;
        private BattleView _battleView;
        private BattlePresenter _battlePresenter;
        private Battle _battle;
        
        public override async Task PreloadAssets(string battleView)
        {
            var handle = Addressables.InstantiateAsync(battleView);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _battleViewObject = handle.Result;
                _battleView = _battleViewObject.GetComponent<BattleView>();
            }
            
            await Task.Delay(100);
        }
        
        public override Task Connect()
        {
            _battlePresenter = new BattlePresenter();
            return Task.CompletedTask;
        }
        
        public async Task Connect(Action restartCallback, Action quitGame, BattleStageData battleStageData)
        {
            await Connect();
            _battle = new Battle(_battlePresenter);
            await _battlePresenter.Connect(_battleView, battleStageData, OnRestartStage, StartPlayerRoll, 
                PlayerMainShot, OnBulletDamage, OnBulletWeaknessSpotDamage, OnBossBattleZoneEnter,
                quitGame);
            await _battle.Connect(battleStageData, restartCallback);
        }

        public override Task Start()
        {
            _battle.Start(Game.GameData.BattleStages[0]);
            
            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            if (_battleView != null)
            {
                _battleView.Clear();
            }

            if (_battleViewObject != null)
            {
                Addressables.ReleaseInstance(_battleViewObject);
                Object.Destroy(_battleViewObject);
            }

            _battleViewObject = null;
            _battleView = null;
            
            return Task.CompletedTask;
        }

        public override async Task MainTick(float deltaTime)
        {
            if (_battle == null)
            {
                return;
            }
            
            _battle.UpdateLogicTick(deltaTime);
            if (_battlePresenter == null)
            {
                return;
            }
            
            await _battlePresenter.MainTick(deltaTime);
            await Task.CompletedTask;
        }
        
        private void OnRestartStage()
        {
            _battle.OnRestartStage();
        }
        
        private bool StartPlayerRoll()
        {
            return _battle.StartPlayerRoll();
        }
        
        private bool PlayerMainShot()
        {
            return _battle.PlayerMainShoot();
        }
        
        private void OnBulletDamage(int bulletId, int targetUnitId)
        {
            _battle.BulletDamage(bulletId, targetUnitId, false);
        }
        
        private void OnBulletWeaknessSpotDamage(int bulletId, int targetUnitId)
        {
            _battle.BulletDamage(bulletId, targetUnitId, true);
        }

        private void OnBossBattleZoneEnter()
        {
            _battle.StartBossFight();
        }
    }
}