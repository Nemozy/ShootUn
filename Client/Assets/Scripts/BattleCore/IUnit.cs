using BattleCore.DamageProjectile;

namespace BattleCore
{
    public interface IUnit
    {
        int Id { get; }
        Team Team { get; }
        float MaxHealth { get; }
        float Health { get; }
        bool IsBoss  { get; }
        int Invincibility  { get; }
        void UpdateLogicTick(float deltaTime);
        void Damage(IDamageProjectile projectile, bool isWeaknessSpotTarget);
        void AddInvincibility();
        void RemoveInvincibility();
        void WakeUp();
    }
}