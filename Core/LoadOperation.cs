using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow.AssetLoader.Core
{
    public class LoadOperation : CustomYieldInstruction
    {
        private AssetBundle _assetbundle;
        private string _assetPath;
        
        public LoadOperation(string assetbundlePath, string assetPath)
        {
            
        }
        
        public T GetAsset<T>() where T : UnityEngine.Object
        {
            return _assetbundle.LoadAsset<T>(_assetPath);
        }
        
        public override bool keepWaiting
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}