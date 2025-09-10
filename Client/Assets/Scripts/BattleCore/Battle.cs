using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleCore.DamageProjectile;
using Configs;

namespace BattleCore
{
    public class Battle : ICoreBattle
    {
        public BattleStatus BattleStatus { get; private set; }
        
        private BattlePresenter _battleViewPresenter;
        private BattleStageData _stageData;
        private Action _restartCallback;
        private int _idCounter;
        private BattlePlayer _battlePlayer;
        private List<IUnit> _enemyUnits = new();
        private List<IDamageProjectile> _damageProjectiles = new();
        
        public Battle(BattlePresenter battleViewPresenter)
        {
            _battleViewPresenter = battleViewPresenter;
            BattleStatus = BattleStatus.Loading;
            Clear();
        }
        
        public async Task Connect(BattleStageData battleStageData, Action restartCallback)
        {
            _stageData = battleStageData;
            _restartCallback = restartCallback;
            _battlePlayer = new BattlePlayer(this, GenerateId(), Game.GameData.Player);
            await _battleViewPresenter.SpawnPlayer(_battlePlayer.Id, _battlePlayer.PlayerData);
            var bossUnit = new BattleUnit(this, battleStageData.Boss, GenerateId(), Team.DARK_SIDE, true);
            _enemyUnits.Add(bossUnit);
            await _battleViewPresenter.SpawnBossUnit(bossUnit.UnitData, bossUnit.Id);
            
            _battleViewPresenter.SetPlayerMaxHp(_battlePlayer.MaxHealth);
            _battleViewPresenter.SetPlayerCurrentHp(_battlePlayer.Health);
            _battleViewPresenter.SetBossMaxHp(bossUnit.MaxHealth);
            _battleViewPresenter.SetBossCurrentHp(bossUnit.Health);
            _battleViewPresenter.SetBossHpBarActive(false);
            bossUnit.AddInvincibility();
        }

        public void Start(BattleStageData stageData)
        {
            _stageData = stageData;
            BattleStatus = BattleStatus.PrepareForStarted;
        }

        public void SetStarted()
        {
            BattleStatus = BattleStatus.Started;
        }

        public void UpdateLogicTick(float deltaTime)
        {
            switch (BattleStatus)
            {
                case BattleStatus.Loading:
                    break;
                case BattleStatus.PrepareForStarted:
                    SetStarted();
                    break;
                case BattleStatus.Started:
                    _battlePlayer.UpdateLogicTick(deltaTime);
                    _enemyUnits.ForEach(x=>x.UpdateLogicTick(deltaTime));
                    if (_battlePlayer.Health <= 0 ||
                        _enemyUnits.All(x => x.Health <= 0))
                    {
                        StopBattle();
                    }
                    break;
                case BattleStatus.PrepareForFinishing:
                    if (_battlePlayer.Health <= 0)
                    {
                        Finish(false);
                        _battleViewPresenter.ShowFailure();
                        break;
                    }
                    if (_enemyUnits.All(x => x.Health <= 0))
                    {
                        Finish(true);
                        _battleViewPresenter.ShowComplete();
                        break;
                    }
                    break;
            }
        }
        
        public void StopBattle()
        {
            BattleStatus = BattleStatus.PrepareForFinishing;
        }
        
        public void Finish(bool won)
        {
            BattleStatus = won ? BattleStatus.FinishedWon : BattleStatus.FinishedLost;
        }
        
        public void OnRestartStage()
        {
            _restartCallback?.Invoke();
        }
        
        public int GenerateId()
        {
            return ++_idCounter;
        }

        public bool StartPlayerRoll()
        {
            return _battlePlayer.StartRoll();
        }
        
        public bool PlayerMainShoot()
        {
            if (_battlePlayer.MainShoot(out var bullet))
            {
                _damageProjectiles.Add(bullet);
                _battleViewPresenter.SpawnPlayerBullet(bullet.Id, bullet.View);
                
                return true;
            }

            return false;
        }
        
        public void UnitShoot(IDamageProjectile bullet, int ownerId)
        {
            _damageProjectiles.Add(bullet);
            _battleViewPresenter.SpawnEnemyBullet(ownerId, bullet.Id, bullet.View);
        }

        public void PlayUnitAnimation(int unitId, string animationName)
        {
            var owner = _enemyUnits.FirstOrDefault(x => x.Id == unitId);
            if (owner == null)
            {
                return;
            }
            
            _battleViewPresenter.PlayUnitAnimation(unitId, animationName);
        }

        public void BulletDamage(int bulletId, int targetUnitId, bool isWeaknessSpotTarget)
        {
            IUnit target = null;
            if (_battlePlayer.Id == targetUnitId)
            {
                target = _battlePlayer;
            }
            else
            {
                target = _enemyUnits.FirstOrDefault(x => x.Id == targetUnitId);
            }
            
            if (target == null)
            {
                return;
            }
            
            var bullet = _damageProjectiles.FirstOrDefault(x => x.Id == bulletId);
            if (bullet == null)
            {
                return;
            }
            
            target.Damage(bullet, isWeaknessSpotTarget);
            if (target.IsBoss)
            {
                _battleViewPresenter.SetBossCurrentHp(target.Health);
            }

            if (target.Team == Team.PLAYER)
            {
                _battleViewPresenter.SetPlayerCurrentHp(target.Health);
            }

            if (target.Health <= 0)
            {
                _battleViewPresenter.PlayUnitAnimation(targetUnitId, "Death1");
            }
        }
        
        public void StartBossFight()
        {
            _battleViewPresenter.SetBossHpBarActive(true);
            var bossUnit = _enemyUnits.FirstOrDefault(x => x.IsBoss);
            if (bossUnit == null)
            {
                return;
            }
            _battleViewPresenter.BossPlayStartPhaseOne(bossUnit.Id);
            bossUnit.RemoveInvincibility();
            bossUnit.WakeUp();
        }
        
        private void Clear()
        {
            _enemyUnits.Clear();
        }
    }
}