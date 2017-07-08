using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow.AssetLoader.Core
{
    public abstract class LoadOperation : CustomYieldInstruction
    {
        protected AsyncOperation Request { get; private set; }

        protected readonly string _assetbundlePath;

        private readonly List<LoadBundleOperation> _dependenciesLoadOperation = new List<LoadBundleOperation>();

        public bool IsDone { get; private set; }

        protected LoadOperation(string assetbundlePath)
        {
            if (!MainLoader.Instance.LoadedBundles.ContainsKey(assetbundlePath))
            {
                _assetbundlePath = assetbundlePath;
                var dependencies = MainLoader.Instance.Manifest.GetAllDependencies(_assetbundlePath);
                foreach (var dependency in dependencies)
                {
                    LoadedBundle loadedBundle;
                    if (MainLoader.Instance.LoadedBundles.TryGetValue(dependency, out loadedBundle))
                    {
                        loadedBundle.ReferecedCount++;
                    }
                    else
                    {
                        _dependenciesLoadOperation.Add(new LoadBundleOperation(dependency));
                    }
                }
            }
            else
            {
                IsDone = true;
            }
        }

        protected abstract AsyncOperation AddLoadRequest();

        protected abstract void LoadDoneMethod();

        public override bool keepWaiting
        {
            get
            {
                if (Request == null)
                {
                    bool isAllDependenciesDone = true;
                    foreach (var operation in _dependenciesLoadOperation)
                    {
                        if (!operation.IsDone)
                        {
                            isAllDependenciesDone = false;
                        }
                    }
                    if (isAllDependenciesDone)
                    {
                        Request = AddLoadRequest();
                    }
                }
                else
                {
                    IsDone = Request.isDone;
                    if (Request.isDone)
                    {
                        LoadDoneMethod();
                    }
                }
                return !IsDone;
            }
        }
    }
}