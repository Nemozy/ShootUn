using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Base;
using UnityEngine;

namespace UI
{
    public static class UI
    {
        private static bool _initialized;
        private static UIAssets _uiAssets;
        private static UIManager _uiManager;
        
        public static Dictionary<Type, (HashSet<BaseUI> free, HashSet<BaseUI> taken)> UiPool => _uiManager.UiPool;
        public static UnityEngine.Camera UiCamera => _uiManager.UiCamera;
        public static GameObject UiRoot => _uiManager.UiRoot;
        public static bool Initialized => _initialized;
        
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            if (_initialized)
            {
                return;
            }

            _uiAssets = LoadAndInitUIAssets();
            _uiManager = new UIManager(_uiAssets);
            _initialized = true;
        }
        
        public static async Task Prewarm<T>() where T : BaseUI, new()
        {
            await _uiManager.Prewarm<T>();
        }
        
        public static T Add<T, TData>(TData data)
            where T : BaseUI<TData>, new()
            where TData : struct
        {
            return _uiManager.Add<T, TData>(data);
        }
        
        public static void Remove<T>(T ui) where T : BaseUI
        {
            _uiManager.Remove(ui);
        }
        
        public static T Open<T, TData>(TData data, bool instant)
            where T : BaseUI<TData>, new()
            where TData : struct
        {
            return _uiManager.Open<T, TData>(data, instant);
        }

        public static void Close<T>(T ui, bool instant) where T : BaseUI
        {
            _uiManager.Close(ui, instant);
        }

        private static UIAssets LoadAndInitUIAssets()
        {
            return AssetsManager.LoadAndInitUIAssets();
        }
    }
}