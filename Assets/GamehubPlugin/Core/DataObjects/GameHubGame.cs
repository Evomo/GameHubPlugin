﻿using System;
using GamehubPlugin.Core.Util;
using GamehubPlugin.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GamehubPlugin.Core {
    [Serializable]
    public class ScorePanelColorScheme {
        [Tooltip("Color used for the pause button and the score overlay background")]
        public Color backgroundColor;
        [Tooltip("Color used for the score trophy icon displayed in the gamehub")]
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

    [Flags]
    public enum RecordType {
        Movements = 1 << 0,
        ElementalMove = 1 << 1
    }

    public enum OverlayType {
        Horizontal,
        Vertical
    }
    
    [Serializable]
    public class OverlayOptions {
        public ScorePanelColorScheme colorScheme = new ScorePanelColorScheme();
        public DeviceOrientation screenOrientation;
        public OverlayType overlayType;
        public bool useOverlay = true; 

    };
    public class GameHubGame : ScriptableObject {
        [Tooltip("which scene should be loaded when starting the game ")]
        public SceneReference mainSceneReference;
        public string gameName;
        public string description;
        [Tooltip("Set Internally ")]
        public int gameId;
        [Tooltip("Which movements should be recorded for a session in this game? Defaults to everything")]
        public RecordType recordType;
        public OverlayOptions overlayOptions = new OverlayOptions();
        [Tooltip("Universal Render Pipeline asset to ensure everything gets rendered correctly, can be left empty")]
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