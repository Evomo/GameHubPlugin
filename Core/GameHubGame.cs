﻿using GamehubPlugin.Util;
using UnityEditor;
using UnityEngine;

namespace GamehubPlugin.Core {

	public class GameHubGame : ScriptableObject {
		public SceneReference mainSceneReference;
		public string gameName;
		public string description;

#if UNITY_EDITOR
		[MenuItem("Evomo/Gamehub/Create Game Asset")]
		public static void CreateGameHubGame() {
			GameHubGame gs = CreateInstance<GameHubGame>();
			gs.gameName = Application.productName;
			AssetDatabase.CreateAsset(gs, $"Assets/{gs.gameName}.asset");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
#endif
	}
}