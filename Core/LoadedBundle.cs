using UnityEngine;

namespace Meow.AssetLoader.Core
{
    public class LoadedBundle
    {
        public AssetBundle AssetBundle;
        public int ReferecedCount;

        public LoadedBundle(AssetBundle assetBundle)
        {
            AssetBundle = assetBundle;
            ReferecedCount = 1;
        }
    }
}