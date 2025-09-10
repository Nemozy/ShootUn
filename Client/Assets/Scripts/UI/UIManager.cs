using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UI
{
    public class UIManager
    {
        public const float TargetScreenWidth = 1920;
        public const float TargetScreenHeight = 1080;
        
        public UnityEngine.Camera UiCamera => _uiCamera;
        public GameObject UiRoot => _uiRoot;
        
        private readonly UnityEngine.Camera _uiCamera;
        private readonly GameObject _uiRoot;
        private readonly GameObject _uiPoolGameObject;
        private readonly UIPool _uiPool;
        
        public Dictionary<Type, (HashSet<BaseUI> free, HashSet<BaseUI> taken)> UiPool => _uiPool.UiPool;

        public UIManager(UIAssets uiAssets)
        {
            _uiCamera = UnityEngine.Camera.allCameras.FirstOrDefault(c=>c.CompareTag("UiCamera"));
            _uiRoot = CreateRoot(_uiCamera);
            _uiPoolGameObject = CreatePoolGameObject();
            _uiPool = new UIPool(_uiRoot.transform, _uiPoolGameObject.transform, uiAssets);
        }
        
        public async Task Prewarm<T>() where T : BaseUI, new()
        {
            await _uiPool.Prewarm<T>();
        }
        
        public T Add<T, TData>(TData data)
            where T : BaseUI<TData>, new()
            where TData : struct
        {
            return Add<T, TData>(data, _uiPool);
        }
        
        public void Remove<T>(T ui) 
            where T : BaseUI
        {
            Remove(ui, _uiPool);
        }
        
        public T Open<T, TData>(TData data, bool instant)
            where T : BaseUI<TData>, new()
            where TData : struct
        {
            return Open<T, TData>(data, instant, _uiPool);
        }

        public void Close<T>(T ui, bool instant) 
            where T : BaseUI
        {
            Close(ui, instant, _uiPool);
        }
        
        private static GameObject CreateRoot(UnityEngine.Camera uiCamera)
        {
            var root = new GameObject("RootUi");
            root.AddComponent<Canvas>();

            var scaler = root.AddComponent<CanvasScaler>();

            UpdateRenderMode(root, uiCamera);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            scaler.referenceResolution = new Vector2(TargetScreenWidth, TargetScreenHeight);
            Object.DontDestroyOnLoad(root);
            return root;
        }
        
        private static GameObject CreatePoolGameObject()
        {
            var pool = new GameObject("PoolUi");
            var scaler = pool.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            scaler.referenceResolution = new Vector2(TargetScreenWidth, TargetScreenHeight);
            Object.DontDestroyOnLoad(pool);
            pool.SetActive(false);
            
            return pool;
        }
        
        private static T Add<T, TData>(TData data, UIPool uiPool)
            where T : BaseUI<TData>, new()
            where TData : struct
        {
            var ui = uiPool.Get<T>();
            var uiWithData = (BaseUI<TData>) ui;
            uiWithData.SetData(data);
            ui.Add();
            
            return ui;
        }
        
        private static void Remove<T>(T ui, UIPool uiPool)
            where T : BaseUI
        {
            ui.Remove();
            uiPool.Release(ui);
        }
        
        private static T Open<T, TData>(TData data, bool instant, UIPool uiPool)
            where T : BaseUI<TData>, new()
            where TData : struct
        {
            var ui = uiPool.Get<T>();
            var uiWithData = (BaseUI<TData>) ui;
            uiWithData.SetData(data);
            ui.Open(true);
            
            return ui;
        }

        private static void Close<T>(T ui, bool instant, UIPool uiPool)
            where T : BaseUI
        {
            ui.Close(true);
            uiPool.Release(ui);
        }
        
        private static void UpdateRenderMode(GameObject root, UnityEngine.Camera uiCamera)
        {
            var canvas = root.GetComponent<Canvas>();

            if (uiCamera != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = uiCamera;
            }
            else
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }
    }
}