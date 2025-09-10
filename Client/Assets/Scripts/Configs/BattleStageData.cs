using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "BattleStageData", menuName = "Tools/ScriptableObjects/BattleStageData")]
    public class BattleStageData : ScriptableObject
    {
        [SerializeField] public string EnvironmentView;
        [SerializeField] public UnitData Boss;
    }
}