using System;
using UnityEngine;
using UnityEngine.UI;
using View;

namespace UI.Base
{
    public abstract class BaseUIView : BaseView
    {
        public bool Initialized { get; private set; }
        
        internal Canvas Canvas { get; private set; }
        internal GraphicRaycaster Raycaster { get; private set; }
        
        public void Init()
        {
            BaseInitialization();
            CreateViewModel();
            OnInit();
        }

        protected void BaseInitialization()
        {
            if (Initialized)
            {
                throw new Exception($"{GetType().Name} already initialized");
            }
            Initialized = true;
            Canvas = GetComponent<Canvas>();
            Raycaster = GetComponent<GraphicRaycaster>();
            Connect(string.Empty);
        }

        protected abstract void CreateViewModel();

        protected abstract void OnInit();

        public virtual void OnAdd()
        {
            
        }
        
        internal void SetActive(bool isActive)
        {
            if (Canvas == null || Raycaster == null)
            {
                return;
            }
            Canvas.enabled = isActive;
            Raycaster.enabled = isActive;
        }
    }
}