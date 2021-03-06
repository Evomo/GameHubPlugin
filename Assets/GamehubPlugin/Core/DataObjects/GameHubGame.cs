﻿using System;
using GamehubPlugin.Core.Util;
using GamehubPlugin.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GamehubPlugin.Core {
    #region Extra Classes

    [Serializable]
    public class ScorePanelColorScheme {
        [Tooltip("Color used for the pause button and the score overlay background")]
        public Color backgroundColor;

        [Tooltip("Color used for the score trophy icon displayed in the gamehub")]
        public Color scoreColor;

        [Tooltip("Color used for the overlay text ")]
        public Color textColor;


        public ScorePanelColorScheme() {
            backgroundColor = Extensions.ToColor("#FFFFFFA3");
            scoreColor = Extensions.ToColor("#3D3673");
            textColor = Extensions.ContrastColor(backgroundColor);
        }
    }

    public enum DeviceOrientation {
        Horizontal,
        Vertical
    }

    [Flags]
    public enum RecordType {
        Movements = 1 << 0,
        ElementalMove = 1 << 1
    }

    [Flags]
    public enum PanelOptions {
        LIVES = 1 << 0,
        COINS = 1 << 1,
        SCORE = 1 << 2
    }

    [Serializable]
    public class OverlayOptions {
        public ScorePanelColorScheme colorScheme = new ScorePanelColorScheme();
        public DeviceOrientation menuType;
        public PanelOptions usedPanels = PanelOptions.COINS | PanelOptions.LIVES | PanelOptions.SCORE;
    };

    #endregion

    public class GameHubGame : ScriptableObject {
        public string gameName;

        [Tooltip("which scene should be loaded when starting the game ")]
        public SceneReference mainSceneReference;

        [Tooltip("Which movements should be recorded for a session in this game? Defaults to everything")]
        public RecordType recordType = RecordType.Movements | RecordType.ElementalMove;

        [Tooltip("Universal Render Pipeline asset to ensure everything gets rendered correctly, can be left empty")]
        public RenderPipelineAsset scriptableRenderAsset;

        public OverlayOptions overlayOptions = new OverlayOptions();
        public DeviceOrientation screenOrientation;

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