namespace BattleCore.UnitLogic
{
    public interface IUnitLogic
    {
        public void OnWakeUp()
        {
        }
        
        public void OnLogicTick(float timeDelta)
        {
        }
        
        public float OnDamage(float receivedDamage)
        {
            return receivedDamage;
        }
    }
}