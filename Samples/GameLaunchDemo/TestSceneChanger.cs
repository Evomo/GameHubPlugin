using System;
using GamehubPlugin.Core;
using UnityEngine;
namespace GamehubPlugin.Samples.GameLaunchDemo {
	public class TestSceneChanger : MonoBehaviour {
		public void Update() {

			if (Input.GetKeyDown(KeyCode.Space)) {
				Debug.Log("start");
				StartGame();
			}

			if (Input.GetKeyDown(KeyCode.A)) {
				Debug.Log("stop");
				StopGame();
			}
		}

		public void StartGame() {
			GameHubManager.LoadScene(1);
		}

		public void StopGame() {
			GameHubManager.StopGame();
		}
	}
}