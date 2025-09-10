using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "UiAssets", menuName = "Tools/UIAssets")]
    public class UIAssets : ScriptableObject
    {
        [Serializable]
        private struct Asset
        {
            public string TypeName;
            public string Key;
        }

        [SerializeField] private List<Asset> _assetsList;
        
        private readonly Dictionary<Type, string> _assetsDictionary = new();
        
        public IReadOnlyDictionary<Type, string> AssetsDictionary => _assetsDictionary;

        internal void Init()
        {
            FillAssetsDictionary(_assetsDictionary, _assetsList);
        }

        private static void FillAssetsDictionary(IDictionary<Type, string> assetsDictionary, IEnumerable<Asset> assetsList)
        {
            assetsDictionary.Clear();
            foreach (var assetTypeAndKey in assetsList)
            {
                var type = Type.GetType(assetTypeAndKey.TypeName);
                if (type != null)
                {
                    assetsDictionary.Add(type, assetTypeAndKey.Key);
                }
                else
                {
                    var asm = Assembly.GetExecutingAssembly();
                    foreach (var exportedType in asm.GetExportedTypes())
                    {
                        if (exportedType.Name != assetTypeAndKey.TypeName)
                        {
                            continue;
                        }
                        type = exportedType;
                        assetsDictionary.Add(type, assetTypeAndKey.Key);
                    }

                    if (type == null)
                    {
                        Debug.LogError("Saved UI type error:" + assetTypeAndKey.TypeName);
                    }
                }
            }
        }

#if UNITY_EDITOR
        public void TryAddAsset(Type assetType, string assetKey)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(assetKey, @"\p{IsCyrillic}"))
            {
                throw new Exception($"Asset key for {assetType.Name} has cyrillic symbols");
            }

            if (_assetsDictionary.Count == 0)
            {
                FillAssetsDictionary(_assetsDictionary, _assetsList);
            }

            if (_assetsDictionary.TryGetValue(assetType, out var savedAssetKey))
            {
                if (savedAssetKey != assetKey)
                {
                    _assetsDictionary[assetType] = assetKey;

                    var typeAssemblyQualifiedName = assetType.AssemblyQualifiedName;
                    var index = _assetsList.FindIndex(x => x.TypeName == typeAssemblyQualifiedName);

                    _assetsList[index] = new Asset
                    {
                        TypeName = assetType.AssemblyQualifiedName,
                        Key = assetKey
                    };

                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssets();
                }
            }
            else
            {
                _assetsDictionary.Add(assetType, assetKey);
                _assetsList.Add(new Asset
                {
                    TypeName = assetType.AssemblyQualifiedName,
                    Key = assetKey
                });
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        [ContextMenu("Reset")]
        private void Reset()
        {
            _assetsDictionary.Clear();
            _assetsList.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}