using System.Collections;
using GamehubPlugin.Util;
using MotionAI.Core;
using MotionAI.Core.Controller;
using MotionAI.Core.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GamehubPlugin.Core {
	public class GameHubManager : Singleton<GameHubManager> {
		private SceneReference _loadedScene;



		private MotionAIManager CurrentManager;
		private Session currSess;

		public UnityEvent OnEndSession, OnStartSession;
		#region Session Handle

		public void StartSession(string gameName) {
			CurrentManager = FindObjectOfType<MotionAIManager>();
			currSess = new Session(gameName);
			CurrentManager.controllerManager.onMovement.AddListener(currSess.RecordMovement);
		}


		public void EndSession(float score = 0) {
			if (currSess != null) {
				currSess?.EndSession(score);
				Debug.Log(currSess.ToString());
				//TODO send to native sdk 
			}

			currSess = null;
		}

		#endregion

		#region Scene Loading

		public void LoadGameScene(SceneReference scene) {
			if (_loadedScene == null) {
				StartCoroutine(LoadGame(scene));
			}
		}
		public void UnloadScene() {
			if (_loadedScene != null) {
				StartCoroutine(UnloadGame());
			}
			else {
				Debug.Log("Would now return to the Gamehub");
			}
		}

		IEnumerator LoadGame(SceneReference scene) {
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
			yield return new WaitUntil(() => asyncLoad.isDone);

	

			SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.ScenePath));
			_loadedScene = scene;

		}


		IEnumerator UnloadGame() {
			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
			yield return new WaitUntil(() => asyncLoad.isDone);
			_loadedScene = null;
		}

		#endregion
	}
}