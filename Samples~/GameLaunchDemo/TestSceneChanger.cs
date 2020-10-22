using System;
using GamehubPlugin.Core;
using GamehubPlugin.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamehubPlugin.Samples.GameLaunchDemo {
	public class TestSceneChanger : MonoBehaviour {
		[Tooltip(
			"Optional scene to test how it'll behave in the gamehub, will load the scene in build index 1 if left null")]
		public SceneReference testScene;

		[SerializeField] private int sceneNum;


		public void Start() {
			sceneNum = testScene == null ? 1 : BuildUtils.GetBuildScene(testScene.sceneAsset).buildIndex;
		}

		public void Update() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				Debug.Log("start");
				GameHubManager.Instance.LoadSceneWrapped(sceneNum);
			}

			if (Input.GetKeyDown(KeyCode.A)) {
				Debug.Log("stop");
				GameHubManager.StopGame();
			}


			if (Input.GetKeyDown(KeyCode.S)) {
				Debug.Log("restarting");
				GameHubManager.ResetGame();
			}
		}
	}
}