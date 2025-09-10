using System;
using System.Collections.Generic;
using UnityEngine;
using View.Battle;
using Random = UnityEngine.Random;

namespace View
{
    public class BattleEnvironmentView : BaseView
    {
        [SerializeField] private List<Transform> _playerSpawn;
        [SerializeField] private Transform _bossSpawn;
        [SerializeField] private StartBattleTriggerView _bossFightTrigger;

        public void Connect(Action enterBattleZoneCallback)
        {
            _bossFightTrigger.Connect(enterBattleZoneCallback);
        }
        
        public Transform GetPlayerSpawn()
        {
            return _playerSpawn[Random.Range(0, _playerSpawn.Count)];
        }
        
        public Transform GetBossSpawn()
        {
            return _bossSpawn;
        }
    }
}