using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Meow.AssetLoader.Core
{
    public class LoadBundleOperation : CustomYieldInstruction
    {
        private WWW _www;

        private LoadBundleOperation _currnetLoadingDependency;

        private readonly string _assetbundleName;

        private readonly Queue<string> _pendingDependencies = new Queue<string>();

        public bool IsDone { get; private set; }

        public LoadBundleOperation(string assetbundleName)
        {
#if UNITY_EDITOR
            if (MainLoader.SimulateAssetBundleInEditor)
            {
                IsDone = true;
            }
            else
#endif
            {
                if (!MainLoader.LoadedBundles.ContainsKey(assetbundleName))
                {
                    IsDone = false;
                    _assetbundleName = assetbundleName;
                    var dependencies = MainLoader.Manifest.GetAllDependencies(_assetbundleName);
                    foreach (var dependency in dependencies)
                    {
                        LoadedBundle loadedBundle;
                        if (MainLoader.LoadedBundles.TryGetValue(dependency, out loadedBundle))
                        {
                            loadedBundle.ReferecedCount++;
                        }
                        else
                        {
                            _pendingDependencies.Enqueue(dependency);
                        }
                    }
                }
                else
                {
                    IsDone = true;
                }
            }
        }

        public T GetAsset<T>(string assetPath) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (MainLoader.SimulateAssetBundleInEditor)
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
#endif
            return _www.assetBundle.LoadAsset<T>(assetPath);
        }

        public override bool keepWaiting
        {
            get
            {
                if (IsDone)
                {
                    return false;
                }

                if (_www == null)
                {
                    if (_currnetLoadingDependency == null || _currnetLoadingDependency.IsDone)
                    {
                        if (_pendingDependencies.Count > 0)
                        {
                            var denpendencyPath = _pendingDependencies.Dequeue();
                            _currnetLoadingDependency = new LoadBundleOperation(denpendencyPath);
                            MainLoader.Instance.StartCoroutine(_currnetLoadingDependency);
                        }
                        else
                        {
                            _www = new WWW(Path.Combine(MainLoader.AssetbundleRootPath, _assetbundleName));
                        }
                    }
                }

                if (_www != null)
                {
                    if (_www.isDone)
                    {
                        MainLoader.LoadedBundles.Add(_assetbundleName, new LoadedBundle(_assetbundleName, _www.assetBundle));
                        IsDone = true;
                    }
                }
                return !IsDone;
            }
        }
    }
}