using System;
using GamehubPlugin.Core;
using UnityEngine;

namespace GamehubPlugin.Samples.GameLaunchDemo {
	public class TestSceneChanger : MonoBehaviour {
		public void Update() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				Debug.Log("start");
				GameHubManager.LoadScene(1);
			}

			if (Input.GetKeyDown(KeyCode.A)) {
				Debug.Log("stop");
				GameHubManager.StopGame();
			}
		}
	}
}