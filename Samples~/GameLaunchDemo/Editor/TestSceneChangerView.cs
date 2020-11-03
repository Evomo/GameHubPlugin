using GamehubPlugin.Core;
using UnityEditor;
using UnityEngine;

namespace GamehubPlugin.Samples.GameLaunchDemo.Editor {
    [CustomEditor(typeof(TestSceneChanger))]
    public class TestSceneChangerView : UnityEditor.Editor {
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
}