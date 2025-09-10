namespace BattleCore.DamageProjectile
{
    public class Bullet : IDamageProjectile
    {
        public Team Team { get; private set; }
        public int Id { get; private set; }
        public int Damage { get; private set; }
        public string View { get; private set; } // TODO: переделать на BulletData
        
        public Bullet(int id, Team team, int damage, string view)
        {
            Id = id;
            Team = team;
            Damage = damage;
            View = view;
        }
    }
}