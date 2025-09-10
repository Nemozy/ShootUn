using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "BossDemonUnitData", menuName = "Tools/ScriptableObjects/BossDemonUnitData")]
    public class BossDemonUnitData : UnitData
    {
        public int HomingBulletDamage = 50;
        public int HomingBulletMoveSpeed = 5;
        public int HomingBulletCooldown = 10;
        public int HomingBulletLifeTime = 10;
        public string HomingBulletView;
        public int HandDamage = 100;
        public float HandDuration = 10;
        public int HandHomingBulletCount = 3;
        public string HandBulletView;
        public int BeamTickDamage = 20;
        public float BeamTickCooldown = 0.5f;
        public float BeamCooldown = 30;
        public float BeamDuration = 8;
        public string BeamBulletView;
    }
}