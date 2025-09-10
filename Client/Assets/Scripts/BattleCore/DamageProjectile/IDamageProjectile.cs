namespace BattleCore.DamageProjectile
{
    public interface IDamageProjectile
    {
        int Id { get; }
        string View { get; }
        Team Team { get; }
        int Damage { get; }
    }
}