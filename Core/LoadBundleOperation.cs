using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow.AssetLoader.Core
{
    public class LoadBundleOperation : CustomYieldInstruction
    {
        private AssetBundleCreateRequest _request;

        private LoadBundleOperation _currnetLoadingDependency;

        private readonly string _assetbundlePath;

        private readonly Queue<string> _pendingDependencies = new Queue<string>();

        public bool IsDone { get; private set; }

        public LoadBundleOperation(string assetbundlePath)
        {
            if (!MainLoader.LoadedBundles.ContainsKey(assetbundlePath))
            {
                _assetbundlePath = assetbundlePath;
                var dependencies = MainLoader.Manifest.GetAllDependencies(_assetbundlePath);
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

        public override bool keepWaiting
        {
            get
            {
                if (_request == null)
                {
                    if (_currnetLoadingDependency == null || _currnetLoadingDependency.IsDone)
                    {
                        if (_pendingDependencies.Count > 0)
                        {
                            var denpendencyPath = _pendingDependencies.Dequeue();
                            _currnetLoadingDependency = new LoadBundleOperation(denpendencyPath);
                        }
                        else
                        {
                            _request = AssetBundle.LoadFromFileAsync(_assetbundlePath);
                        }
                    }
                }

                if (_request != null)
                {
                    if (_request.isDone)
                    {
                        MainLoader.LoadedBundles.Add(_assetbundlePath, new LoadedBundle(_assetbundlePath, _request.assetBundle));
                        IsDone = true;
                    }
                }
                return !IsDone;
            }
        }
    }
}