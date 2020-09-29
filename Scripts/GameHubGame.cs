using GamehubPlugin.Util;
using UnityEditor;
using UnityEngine;

namespace GamehubPlugin {
	public class GameLauncherData {
		public SceneReference mainSceneReference;
		public string gameName;
		public Texture2D image;
		public string description;
		public string developerName;
	}

	public class GameHubGame : ScriptableObject {
		public SceneReference mainSceneReference;
		public string gameName;
		public Texture2D image;
		public string description;
		public string developerName;


#if UNITY_EDITOR
		[MenuItem("Evomo/Gamehub/Create Game Asset")]
		public static void CreateSceneObjects() {
			GameHubGame gs = CreateInstance<GameHubGame>();
			gs.gameName = Application.productName;
			AssetDatabase.CreateAsset(gs, $"Assets/{gs.gameName}.asset");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
#endif

		public GameLauncherData GetGameLauncherData() {
			GameLauncherData gld = new GameLauncherData();

			gld.mainSceneReference = mainSceneReference;
			gld.gameName = gameName;
			gld.image = image;
			gld.description = description;
			gld.developerName = developerName;

			return gld;
		}
	}
}