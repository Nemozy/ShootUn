using System;
using UnityEngine;

namespace View.Battle
{
    public class UnitBulletConnector : MonoBehaviour
    {
        public int Id { get; private set; }
        public Transform Target { get; private set; }
        public Transform Owner { get; private set; }
        public int OwnerUnitId { get; private set; }
        public Action<int, PlayerUnitView> HitPlayerCallback { get; private set; }
        
        public Action OnConnect;
        
        public void Connect(int id, Action<int, PlayerUnitView> hitPlayerCallback, Transform mainTarget, Transform owner,
            int ownerUnitId)
        {
            Id = id;
            OwnerUnitId = ownerUnitId;
            HitPlayerCallback = hitPlayerCallback;
            Target = mainTarget;
            Owner = owner;
            OnConnect?.Invoke();
        }
    }
}