using System;
using GamehubPlugin.Core.Util;
using GamehubPlugin.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GamehubPlugin.Core {
    [Serializable]
    public class ScorePanelColorScheme {
        public Color backgroundColor;
        public Color scoreColor;

        public ScorePanelColorScheme() {
            backgroundColor = Extensions.ToColor("#FFFFFFA3");
            scoreColor = Extensions.ToColor("#3D3673");
        }
    }

    public enum DeviceOrientation {
        Landscape,
        Portrait
    }

    public class GameHubGame : ScriptableObject {
        public SceneReference mainSceneReference;
        public string gameName;
        public string description;
        public bool recordElmos;
        public int gameId;
        public DeviceOrientation screenOrientation;
        public ScorePanelColorScheme colors = new ScorePanelColorScheme();


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