﻿using GamehubPlugin;
using GamehubPlugin.Core;
using GamehubPlugin.Util;
using UnityEngine;

namespace Demos.GameLaunchDemo {
	public class TestSceneChanger : MonoBehaviour {
		public SceneReference secondScene;


		public void LoadGame() {
			GameHubManager.Instance.LoadGameScene(secondScene);
		}


		public void UnloadGame() {
			GameHubManager.Instance.UnloadScene();
		}

	}
}