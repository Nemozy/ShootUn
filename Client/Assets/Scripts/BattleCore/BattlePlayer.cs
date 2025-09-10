using BattleCore.DamageProjectile;
using Configs;
using UnityEngine;

namespace BattleCore
{
    public class BattlePlayer : IUnit
    {
        public Team Team => Team.PLAYER; 
        public int Id { get; private set; }
        public PlayerData PlayerData { get; private set; }
        public float MaxHealth { get; private set; }
        public float Health { get; private set; }
        public int Invincibility { get; private set; }
        public bool IsBoss => false;
        
        private BattlePlayerWeaponManager _weaponManager;
        private float _rollCooldown = 0f;
        private float _rollDuration = 0f;
        private float _rollCooldownTimer = 0f;
        private float _rollInvincibilityTime = 0f;
        private float _invincibilityTimer = 0f;
        
        public BattlePlayer(ICoreBattle battle, int id, PlayerData playerData)
        {
            Id = id;
            _weaponManager = new BattlePlayerWeaponManager(battle, playerData.Weapon);
            PlayerData = playerData;
            MaxHealth = Health = playerData.Health;
            _rollInvincibilityTime = playerData.RollInvincibilityTime;
            _rollCooldown = playerData.RollCooldown;
            _rollDuration = playerData.RollDuration;
        }

        //rollCooldownTimer = rollCooldown;
        
        public void UpdateLogicTick(float deltaTime)
        {
            if (Health <= 0)
            {
                return;
            }
            _weaponManager.UpdateLogicTick(deltaTime);

            if (_rollCooldownTimer > 0)
            {
                _rollCooldownTimer -= deltaTime;
            }
            
            if (_invincibilityTimer > 0)
            {
                _invincibilityTimer -= deltaTime;
                if (_invincibilityTimer <= 0)
                {
                    RemoveInvincibility();
                }
            }
        }

        public bool StartRoll()
        {
            if (_rollCooldownTimer > 0)
            {
                return false;
            }
            
            _rollCooldownTimer = _rollCooldown + _rollDuration;
            _invincibilityTimer = _rollInvincibilityTime;
            AddInvincibility();
            
            return true;
        }
        
        public bool MainShoot(out Bullet bullet)
        {
            return _weaponManager.MainShoot(out bullet);
        }

        public void Damage(IDamageProjectile projectile, bool isWeaknessSpotTarget)
        {
            if (Invincibility > 0)
            {
                return;
            }

            float receivedDamage = isWeaknessSpotTarget ? projectile.Damage * 2 : projectile.Damage;
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
        }
    }
}