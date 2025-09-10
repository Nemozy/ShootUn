using BattleCore.DamageProjectile;
using Configs;

namespace BattleCore
{
    public class BattlePlayerMainGun
    {
        private ICoreBattle _battle;
        private WeaponData _mainWeapon;
        private float _cooldown;
        private float _cooldownTimer;

        public BattlePlayerMainGun(ICoreBattle battle, WeaponData weaponData)
        {
            _battle = battle;
            _mainWeapon = weaponData;
            _cooldown = weaponData.BulletCooldown;
        }
        
        public void UpdateLogicTick(float deltaTime)
        {
            if (_cooldownTimer > 0)
            {
                _cooldownTimer -= deltaTime;
            }
        }

        public bool Shoot(out Bullet bullet)
        {
            if (_cooldownTimer > 0)
            {
                bullet = null;
                return false;
            }

            _cooldownTimer = _cooldown;
            bullet = new Bullet(_battle.GenerateId(), Team.PLAYER, _mainWeapon.BulletDamage, _mainWeapon.BulletView);
            return true;
        }
    }
}