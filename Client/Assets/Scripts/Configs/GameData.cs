using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Tools/ScriptableObjects/GameData")]
    public class GameData : ScriptableObject
    {
        [SerializeField] public List<BattleStageData> BattleStages;
        [SerializeField] public PlayerData Player;
    }
}