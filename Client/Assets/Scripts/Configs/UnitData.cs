using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "Tools/ScriptableObjects/UnitData")]
    public class UnitData : ScriptableObject
    {
        public int Health = 3000;
        public float WalkSpeed = 0f;
        public float RunSpeed = 0f;
        public string UnitView;
    }
}