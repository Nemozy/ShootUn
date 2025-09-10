using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UI.Base;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI
{
    public class UIPool
    {
        private readonly Transform _root;
        private readonly Transform _pool;
        private readonly UIAssets _uiAssets;
        private static readonly SemaphoreSlim Semaphore = new(1);
        
        private readonly Dictionary<Type, (HashSet<BaseUI> free, HashSet<BaseUI> taken)> _uiPool = new();

        public Dictionary<Type, (HashSet<BaseUI> free, HashSet<BaseUI> taken)> UiPool => _uiPool;

        public UIPool(Transform root, Transform pool, UIAssets uiAssets)
        {
            _root = root;
            _pool = pool;
            _uiAssets = uiAssets;
        }
        
        public async Task Prewarm<T>() where T : BaseUI, new()
        {
            await Semaphore.WaitAsync();
            await Prewarm(typeof(T), _uiAssets);
            Semaphore.Release();
        }
        
        public T Get<T>() where T : BaseUI, new()
        {
            return Get<T>(_uiPool, _root, _uiAssets);
        }
        
        public void Release<T>(T ui) where T : BaseUI
        {
            Release(ui, _pool, _uiPool);
        }
        
        private static T Get<T>(
            IDictionary<Type, (HashSet<BaseUI> free, HashSet<BaseUI> taken)> uiPool,
            Transform root,
            UIAssets uiAssets)
            where T : BaseUI, new()
        {
            if (uiPool.TryGetValue(typeof(T), out var uiCollection))
            {
                if (uiCollection.free.Count > 0)
                {
                    var uiFromPool = uiCollection.free.First();
                    uiCollection.free.Remove(uiFromPool);
                    uiCollection.taken.Add(uiFromPool);
                    uiFromPool.BaseUIView.SetActive(true);
                    uiFromPool.BaseUIView.transform.SetParent(root);
                    Semaphore.Release();
                    
                    return (T) uiFromPool;
                }
            }

            var ui = CreateUI<T>(root, uiAssets);
            if (uiCollection.taken != null)
            {
                uiCollection.taken.Add(ui);
            }
            else
            {
                uiPool.Add(typeof(T), (new HashSet<BaseUI>(), new HashSet<BaseUI>
                {
                    ui
                }));
            }
            
            ui.BaseUIView.SetActive(true);
            return ui;
        }
        
        private static void Release<T>(
            T ui, 
            Transform pool,
            IReadOnlyDictionary<Type, (HashSet<BaseUI> free, HashSet<BaseUI> taken)> uiPool)
            where T : BaseUI
        {
            if (uiPool.TryGetValue(typeof(T), out var uiCollection))
            {
                uiCollection.taken.Remove(ui);
                uiCollection.free.Add(ui);
                ui.BaseUIView.SetActive(false);
                if (pool != null && ui.BaseUIView.transform != null)
                {
                    ui.BaseUIView.transform.SetParent(pool);
                }
            }
            else
            {
                throw new Exception("Try to release unknown type UI");
            }
        }
        
        private static T CreateUI<T>(Transform root, UIAssets uiAssets) where T : BaseUI, new()
        {
            var ui = new T();
            var uiPrefab = LoadScreenPrefab(typeof(T), uiAssets);
            if (uiPrefab != null)
            {
                var uiViewInstance = UnityEngine.Object.Instantiate(uiPrefab, root);
                var uiView = uiViewInstance.GetComponent<BaseUIView>();
                if (uiView != null)
                {
                    uiView.Init();
                    if (uiView.Canvas != null)
                    {
                        uiView.Canvas.enabled = false;
                    }
                    ui.ConnectView(uiView);

                    return ui;
                }

                throw new Exception($"UI view not found ({typeof(T)})");
            }

            throw new Exception($"UI prefab loading failed, for type({typeof(T)}");
        }

        private static GameObject LoadScreenPrefab(Type type, UIAssets uiAssets)
        {
            if (uiAssets.AssetsDictionary.TryGetValue(type, out var assetKey))
            {
                var asyncLoadingAssetHandle = Addressables.LoadAssetAsync<GameObject>(assetKey);
                
                return asyncLoadingAssetHandle.WaitForCompletion();
            }

            throw new Exception($"Asset key not found for type ({type})");
        }
        
        private static async Task Prewarm(Type type, UIAssets uiAssets)
        {
            if (uiAssets.AssetsDictionary.TryGetValue(type, out var assetKey))
            {
                var asyncLoadingAssetHandle = Addressables.LoadAssetAsync<GameObject>(assetKey);
                await asyncLoadingAssetHandle.Task;
                return;
            }
            
            throw new Exception($"Asset key not found for type ({type})");
        }
    }
}