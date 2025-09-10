using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Tools/ScriptableObjects/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public float BulletCooldown = 0.2f;
        public int BulletDamage = 10;
        public string BulletView;
    }
}