using BattleCore.DamageProjectile;

namespace BattleCore
{
    public interface ICoreBattle
    {
        int GenerateId();
        void UnitShoot(IDamageProjectile bullet, int ownerId);
        void PlayUnitAnimation(int unitId, string animationName);
    }
}