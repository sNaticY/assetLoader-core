using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Meow.AssetLoader.Core
{
    public class MainLoader : MonoBehaviour
    {
        #region MonoSingletonImplement

        private static MainLoader _instance;

        public static string AssetbundleRootPath;

        public static MainLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = GameObject.Find("MainLoader");
                    if (go == null)
                    {
                        go = new GameObject("MainLoader");
                    }
                    _instance = go.GetComponent<MainLoader>();
                    if (_instance == null)
                    {
                        _instance = go.AddComponent<MainLoader>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        #endregion

        public static AssetBundleManifest Manifest;
        
        public static readonly Dictionary<string, LoadedBundle> LoadedBundles = new Dictionary<string, LoadedBundle>();
        
        
#if UNITY_EDITOR
        static int m_SimulateAssetBundleInEditor = -1;
        
        public static bool SimulateAssetBundleInEditor
        {
            get
            {
                if (m_SimulateAssetBundleInEditor == -1)
                    m_SimulateAssetBundleInEditor = UnityEditor.EditorPrefs.GetBool("SimulateAssetBundles", true) ? 1 : 0;

                return m_SimulateAssetBundleInEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != m_SimulateAssetBundleInEditor)
                {
                    m_SimulateAssetBundleInEditor = newValue;
                    UnityEditor.EditorPrefs.SetBool("SimulateAssetBundles", value);
                }
            }
        }
#endif
        
        public static IEnumerator Initialize(string assetbundleRootPath, string manifeestName)
        {
            AssetbundleRootPath = assetbundleRootPath;
            var manifestBundle = new LoadManifestOperation(Path.Combine(assetbundleRootPath, manifeestName));
            yield return manifestBundle;
            Manifest = manifestBundle.Manifest;
        }

        public static LoadBundleOperation LoadBundle(string bundlePath)
        {
            return new LoadBundleOperation(bundlePath);
        }

        public static LoadLevelOperation LoadLevel(string bundlePath, string levelName, bool isAddtive)
        {
            return new LoadLevelOperation(bundlePath, levelName, isAddtive);
        }
    }
}