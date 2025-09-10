using UnityEngine;

namespace View
{
    public abstract class BaseView : MonoBehaviour
    {
        public string EntityType { get; private set; }
        
        public void Connect(string entityType)
        {
            EntityType = entityType;
        }
    }
}