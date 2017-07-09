using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meow.AssetLoader.Core
{
    public class LoadLevelOperation : CustomYieldInstruction
    {
        private LoadBundleOperation _loadBundleOperation;
        private AsyncOperation _loadLevelOperation;
        
        private readonly string _levelName;
        private readonly bool _isAddtive;

        public bool IsDone { get; private set; }
        
        public LoadLevelOperation(string assetbundlePath, string levelName, bool isAdditive)
        {
            _levelName = levelName;
            _isAddtive = isAdditive;
            _loadBundleOperation = new LoadBundleOperation(assetbundlePath);
            IsDone = false;
        }

        public override bool keepWaiting
        {
            get
            {
                if (_loadBundleOperation.IsDone)
                {
                    if (_loadLevelOperation == null)
                    {
                        _loadLevelOperation = SceneManager.LoadSceneAsync(_levelName, _isAddtive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                    }
                    if (_loadLevelOperation.isDone)
                    {
                        IsDone = true;
                    }
                }
                return !IsDone;
            }
        }
    }
}