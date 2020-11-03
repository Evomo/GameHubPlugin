using System.Collections;
using System.Collections.Generic;
using GamehubPlugin.Core;
using GamehubPlugin.Samples.GameLaunchDemo;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(TestSceneChanger))]
public class TestSceneChangerview : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        TestSceneChanger comp = (TestSceneChanger) target;


        if (comp.testAsset) {
            GameHubGame game = comp.testAsset;
            if (GUILayout.Button($"Launch {game.name}")) {
                comp.LoadGame();
            }

            if (GUILayout.Button($"Reset {game.name}")) {
                comp.Restart();
            }

            if (GUILayout.Button($"Stop {game.name}")) {
                comp.StopGame();
            }
        }

        DrawDefaultInspector();
    }
}