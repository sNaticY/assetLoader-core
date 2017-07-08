using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meow.AssetLoader.Core
{
    public class LoadBundleOperation : LoadOperation
    {
        private AssetBundleCreateRequest Request
        {
            get { return base.Request as AssetBundleCreateRequest; }
        }
        
        public LoadBundleOperation(string assetbundlePath) : base(assetbundlePath)
        {
        }

        public T GetAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            return Request.assetBundle.LoadAsset<T>(assetPath);
        }

        protected override AsyncOperation AddLoadRequest()
        {
            return AssetBundle.LoadFromFileAsync(_assetbundlePath);
        }

        protected override void LoadDoneMethod()
        {
            MainLoader.Instance.LoadedBundles.Add(_assetbundlePath, new LoadedBundle(_assetbundlePath, Request.assetBundle));
        }
    }
}