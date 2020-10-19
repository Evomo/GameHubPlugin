using GamehubPlugin.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GamehubPlugin.Core {
	public class GameHubGame : ScriptableObject {
		public SceneReference mainSceneReference;
		public string gameName;
		public string description;
		public RenderPipelineAsset scriptableRenderAsset;

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