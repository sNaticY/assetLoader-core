using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meow.AssetLoader.Core
{
    public class LoadLevelOperation : LoadOperation
    {
        private readonly string _levelName;
        private readonly bool _isAddtive;
        
        public LoadLevelOperation(string assetbundlePath, string levelName, bool isAdditive) : base(assetbundlePath)
        {
            _levelName = levelName;
            _isAddtive = isAdditive;
        }

        protected override AsyncOperation AddLoadRequest()
        {
            return SceneManager.LoadSceneAsync(_levelName, _isAddtive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        }

        protected override void LoadDoneMethod()
        {
            // LoadLevelDoNotiongWhenLoadingDone
        }
    }
}