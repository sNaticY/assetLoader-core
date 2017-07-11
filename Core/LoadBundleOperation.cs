﻿using System.Collections;
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
            _assetbundleName = assetbundleName;
            
#if UNITY_EDITOR
            if (MainLoader.IsSimulationMode)
            {
                IsDone = true;
            }
            else
#endif
            {
                if (!MainLoader.LoadedBundles.ContainsKey(assetbundleName))
                {
                    IsDone = false;
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
            if (MainLoader.IsSimulationMode)
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
#endif
            return _www.assetBundle.LoadAsset<T>(assetPath);
        }

        public Dictionary<string, T> GetAllAssets<T>() where T : UnityEngine.Object
        {
            Dictionary<string, T> result = new Dictionary<string, T>();
#if UNITY_EDITOR
            if (MainLoader.IsSimulationMode)
            {
                var assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(_assetbundleName);
                foreach (var path in assetPaths)
                {
                    T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    result.Add(path, asset);
                }
            }
            else
#endif
            {
                var assetPaths = _www.assetBundle.GetAllAssetNames();
                foreach (var path in assetPaths)
                {
                    T asset = _www.assetBundle.LoadAsset<T>(path);
                    result.Add(path, asset);
                }
            }
            return result;
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