using System;
using GamehubPlugin.Core;
using GamehubPlugin.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamehubPlugin.Samples.GameLaunchDemo {
    public class TestSceneChanger : MonoBehaviour {
#if UNITY_EDITOR


        public GameHubGame testAsset;

        public void LoadGame() {
            GameHubManager.Instance.LoadSceneWrapped(testAsset);
        }

        public void Restart() {
            GameHubManager.ResetGame();
        }


        public void StopGame() {
            GameHubManager.StopGame();
        }
#endif
    }
}