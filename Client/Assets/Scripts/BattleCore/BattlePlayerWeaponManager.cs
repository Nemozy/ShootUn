using BattleCore.DamageProjectile;
using Configs;

namespace BattleCore
{
    public class BattlePlayerWeaponManager
    {
        private BattlePlayerMainGun _mainWeapon;
        
        public BattlePlayerWeaponManager(ICoreBattle battle, WeaponData weaponData)
        {
            _mainWeapon = new BattlePlayerMainGun(battle, weaponData);
        }
        
        public void UpdateLogicTick(float deltaTime)
        {
            _mainWeapon.UpdateLogicTick(deltaTime);
        }

        public bool MainShoot(out Bullet bullet)
        {
            return _mainWeapon.Shoot(out bullet);
        }
    }
}