using System;
using UnityEngine;

namespace View.Battle
{
    public class StartBattleTriggerView : BaseView
    {
        private Action _enterBattleZoneCallback;
        
        public void Connect(Action enterBattleZoneCallback)
        {
            _enterBattleZoneCallback = enterBattleZoneCallback;
        }

        private void Start()
        {
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("PlayerUnit"))
            {
                return;
            }
            
            _enterBattleZoneCallback?.Invoke();
            gameObject.SetActive(false);
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("PlayerUnit"))
            {
                return;
            }
            
            _enterBattleZoneCallback?.Invoke();
            gameObject.SetActive(false);
        }
    }
}