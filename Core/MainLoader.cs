using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow.AssetLoader.Core
{
    public class MainLoader : MonoBehaviour
    {
        #region MonoSingletonImplement

        private static MainLoader _instance;

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

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}