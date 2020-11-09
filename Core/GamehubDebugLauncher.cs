using System;
using UnityEngine;

namespace GamehubPlugin.Core {
    [RequireComponent(typeof(GameHubManager))]
    public class GamehubDebugLauncher : MonoBehaviour
    {
#if UNITY_EDITOR

        public GameHubGame testAsset;

        private void Start() {
            GameHubManager.Instance.LoadSceneWrapped(testAsset);

        }


#endif
    }
}
