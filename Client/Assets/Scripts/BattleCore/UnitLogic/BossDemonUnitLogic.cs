using BattleCore.DamageProjectile;
using Configs;

namespace BattleCore.UnitLogic
{
    public class BossDemonUnitLogic : IUnitLogic
    {
        enum Phase
        {
            Sleep = -1,
            Phase1 = 0,
            Phase2 = 1
        }
        
        private Phase _currentPhase;
        private ICoreBattle _battle;
        private IUnit _unit;

        private readonly int _homingBulletDamage;
        private readonly int _homingBulletCooldown;
        private readonly int _homingBulletLifetime;
        private readonly string _homingBulletView;
        private float _homingBulletTimer;
        
        private readonly int _handDamage;
        private readonly float _handDuration;
        private readonly int _handHomingBulletCount;
        private readonly string _handBulletView;
        private float _handHomingBulletCounter;

        private readonly int _beamTickDamage;
        private readonly float _beamCooldown;
        private readonly float _beamDuration;
        private readonly string _beamBulletView;
        private float _beamTimer;

        private const float _globalAbilitiesCooldown = 2;
        private float _globalAbilitiesTimer;
        private float _phase2Threshold = 0.5f;
        
        public BossDemonUnitLogic(ICoreBattle battle, IUnit unit, BossDemonUnitData bossDemonUnitData)
        {
            _currentPhase = Phase.Sleep;
            _battle = battle;
            _unit = unit;
            _homingBulletDamage = bossDemonUnitData.HomingBulletDamage;
            _homingBulletCooldown = bossDemonUnitData.HomingBulletCooldown;
            _homingBulletLifetime = bossDemonUnitData.HomingBulletLifeTime;
            _homingBulletView = bossDemonUnitData.HomingBulletView;
            _handDamage = bossDemonUnitData.HandDamage;
            _handDuration = bossDemonUnitData.HandDuration;
            _handHomingBulletCount = bossDemonUnitData.HandHomingBulletCount;
            _handBulletView = bossDemonUnitData.HandBulletView;
            _beamTickDamage = bossDemonUnitData.HandDamage;
            _beamCooldown = bossDemonUnitData.HandHomingBulletCount;
            _beamTickDamage = bossDemonUnitData.BeamTickDamage;
            _beamCooldown = bossDemonUnitData.BeamCooldown;
            _beamDuration = bossDemonUnitData.BeamDuration;
            _beamBulletView = bossDemonUnitData.BeamBulletView;
            
            _homingBulletTimer = 2;
            _handHomingBulletCounter = 0;
            _globalAbilitiesTimer = 0;
        }

        void IUnitLogic.OnWakeUp()
        {
            _currentPhase = Phase.Phase1;
        }
        
        void IUnitLogic.OnLogicTick(float timeDelta)
        {
            if (_currentPhase == Phase.Sleep)
            {
                return;
            }
            
            if (_globalAbilitiesTimer > 0)
            {
                _globalAbilitiesTimer -= timeDelta;
            }
            
            if (_beamTimer > 0)
            {
                _beamTimer -= timeDelta;
            }
            
            if (_homingBulletTimer > 0)
            {
                _homingBulletTimer -= timeDelta;
            }

            if (_globalAbilitiesTimer > 0)
            {
                return;
            }
            
            if (IsBeamAbilityReady())
            {
                UseBeamAbility();
                return;
            }

            if (IsHandAbilityReady())
            {
                UseHandAbility();
                return;
            }

            if (IsHomingBulletAbilityReady())
            {
                UseHomingBulletAbility();
                return;
            }
        }
        
        float IUnitLogic.OnDamage(float receivedDamage)
        {
            if (_currentPhase == Phase.Phase1 && _unit.Health / _unit.MaxHealth <= _phase2Threshold)
            {
                _currentPhase = Phase.Phase2;
            }
            
            return receivedDamage;
        }
        
        //TODO: переделать абилки на фабрику абилок и использовать через компонентную систему
        private bool IsHomingBulletAbilityReady()
        {
            return _homingBulletTimer <= 0;
        }
        
        private void UseHomingBulletAbility()
        {
            _battle.PlayUnitAnimation(_unit.Id, "attack1");
            _battle.UnitShoot(new Bullet(_battle.GenerateId(), _unit.Team, _homingBulletDamage, _homingBulletView), _unit.Id);
            _homingBulletTimer = _homingBulletCooldown;
            _handHomingBulletCounter += 1;
            _globalAbilitiesTimer = _globalAbilitiesCooldown;
        }
        
        private bool IsHandAbilityReady()
        {
            return _handHomingBulletCounter >= _handHomingBulletCount;
        }
        
        private void UseHandAbility()
        {
            _battle.PlayUnitAnimation(_unit.Id, "attack2");
            _battle.UnitShoot(new Bullet(_battle.GenerateId(), _unit.Team, _handDamage, _handBulletView), _unit.Id);
            _handHomingBulletCounter = 0;
            _globalAbilitiesTimer = _globalAbilitiesCooldown + _handDuration;
        }
        
        private bool IsBeamAbilityReady()
        {
            return _currentPhase == Phase.Phase2 && _beamTimer <= 0;
        }
        
        private void UseBeamAbility()
        {
            _battle.PlayUnitAnimation(_unit.Id, "attack3");
            _battle.UnitShoot(new Bullet(_battle.GenerateId(), _unit.Team, _beamTickDamage, _beamBulletView), _unit.Id);
            _beamTimer = _beamCooldown + _beamDuration;
            _globalAbilitiesTimer = _globalAbilitiesCooldown + _beamDuration;
        }
    }
}