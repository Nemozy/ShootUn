using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Tools/ScriptableObjects/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public int Health = 500;
        public float WalkSpeed = 5f;
        public float RunSpeed = 8f;
        public float RollDistance = 4f;
        public float RollDuration = 0.6f;
        public float RollCooldown = 1f;
        public float RollInvincibilityTime = 0.4f;
        public string UnitView;
        public WeaponData Weapon;
    }
}