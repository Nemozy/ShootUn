using System;
using System.Threading.Tasks;
using Configs;
using View;

namespace BattleCore
{
    public class BattlePresenter
    {
        private BattleView _battleView;
        
        public async Task Connect(BattleView battleView, BattleStageData battleStageData, Action restartCallback, 
            Func<bool> startRollCallback, Func<bool> playerMainShotCallback, Action<int, int> bulletDamageCallback, 
            Action<int, int> bulletWeaknessSpotDamageCallback, Action startBossBattleCallback, Action quitGameCallback)
        {
            _battleView = battleView;
            await _battleView.Connect(battleStageData, restartCallback, startRollCallback, playerMainShotCallback, 
                bulletDamageCallback, bulletWeaknessSpotDamageCallback, startBossBattleCallback, quitGameCallback);
        }

        public async Task MainTick(float deltaTime)
        {
        }
        
        public async Task SpawnPlayer(int unitId, PlayerData playerData)
        {
            await _battleView.SpawnPlayer(unitId, playerData);
        }
        
        public async Task SpawnBossUnit(UnitData unitData, int unitId)
        {
            await _battleView.SpawnBossUnit(unitData, unitId);
        }
        
        public void SetPlayerMaxHp(float playerMaxHp)
        {
            _battleView.SetPlayerMaxHp(playerMaxHp);
        }

        public void SetPlayerCurrentHp(float playerCurrentHp)
        {
            _battleView.SetPlayerCurrentHp(playerCurrentHp);
        }
        
        public void SetBossCurrentHp(float bossCurrentHp)
        {
            _battleView.SetBossCurrentHp(bossCurrentHp);
        }
        
        public void SetBossMaxHp(float bossMaxHp)
        {
            _battleView.SetBossMaxHp(bossMaxHp);
        }
        
        public void SetBossHpBarActive(bool isHpBarActive)
        {
            _battleView.SetBossHpBarActive(isHpBarActive);
        }
        
        public void SpawnPlayerBullet(int bulletId, string bulletView)
        {
            _battleView.SpawnPlayerBullet(bulletId, bulletView);
        }
        
        public void SpawnEnemyBullet(int ownerUnitId, int bulletId, string bulletView)
        {
            _battleView.SpawnEnemyBullet(ownerUnitId, bulletId, bulletView);
        }
        
        public void BossPlayStartPhaseOne(int unitId)
        {
            _battleView.BossPlayStartPhaseOne(unitId);
        }

        public void PlayUnitAnimation(int unitId, string animationName)
        {
            _battleView.PlayUnitAnimation(unitId, animationName);
        }
        
        public void ShowComplete()
        {
            _battleView.ShowComplete();
        }

        public void ShowFailure()
        {
            _battleView.ShowFailure();
        }
    }
}