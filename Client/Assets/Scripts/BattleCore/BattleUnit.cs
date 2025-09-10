using BattleCore.DamageProjectile;
using BattleCore.UnitLogic;
using Configs;
using UnityEngine;

namespace BattleCore
{
    public class BattleUnit : IUnit
    {
        public Team Team  { get; private set; }
        public UnitData UnitData { get; private set; }
        public int Id { get; private set; }
        public bool IsBoss  { get; private set; }
        public float MaxHealth { get; private set; }
        public float Health { get; private set; }
        public int Invincibility { get; private set; }

        private IUnitLogic _unitLogic;
        
        public BattleUnit(ICoreBattle battle, UnitData unitData, int id, Team team, bool isBoss)
        {
            Id = id;
            IsBoss = isBoss;
            Team = team;
            UnitData = unitData;
            MaxHealth = unitData.Health;
            Health = unitData.Health;
            Invincibility = 0;

            _unitLogic = UnitLogicFactory.CreateLogic(this, battle, unitData);
        }
        
        public void UpdateLogicTick(float deltaTime)
        {
            if (Health <= 0)
            {
                return;
            }
            _unitLogic.OnLogicTick(deltaTime);
        }

        public void Damage(IDamageProjectile projectile, bool isWeaknessSpotTarget)
        {
            if (Invincibility > 0)
            {
                return;
            }
            float receivedDamage = isWeaknessSpotTarget ? projectile.Damage * 3 : projectile.Damage;
            receivedDamage = _unitLogic.OnDamage(receivedDamage);
            receivedDamage = Mathf.Min(Health, receivedDamage);
            Health -= receivedDamage;
        }

        public void AddInvincibility()
        {
            Invincibility += 1;
        }

        public void RemoveInvincibility()
        {
            Invincibility -= 1;
        }

        public void WakeUp()
        {
            _unitLogic.OnWakeUp();
        }
    }
}