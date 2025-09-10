using Configs;

namespace BattleCore.UnitLogic
{
    public static class UnitLogicFactory
    {
        public static IUnitLogic CreateLogic(IUnit unit, ICoreBattle battle, UnitData unitData)
        {
            switch (unitData)
            {
                case BossDemonUnitData bossDemonUnitData:
                    return new BossDemonUnitLogic(battle, unit, bossDemonUnitData);
                default:
                    return new UnitLogic(battle, unit, unitData);
            }
        }
    }
}