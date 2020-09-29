using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GamehubPlugin.Util;
using MotionAI.Core;
using MotionAI.Core.Controller;
using MotionAI.Core.POCO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamehubPlugin {
	public class GameHubManager : Singleton<GameHubManager> {
		private SceneReference _loadedScene;
		public GameObject currentlyRunningGame;


		private MotionAIManager CurrentManager;
		private Session currSess;

		#region Session Handle

		public void StartSession(string gameName) {
			CurrentManager = FindObjectOfType<MotionAIManager>();
			currSess = new Session(gameName);
			CurrentManager.controllerManager.onMovement.AddListener(currSess.RecordMovement);
		}


		public void EndSession(float score = 0) {
			if (currSess != null ) {
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
			if (currentlyRunningGame != null) {
				if (_loadedScene != null) {
					StartCoroutine(UnloadGame());
				}
			}
			else {
				Debug.Log("Would now return to the Gamehub");
			}
		}


		IEnumerator LoadGame(SceneReference scene) {
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
			yield return new WaitUntil(() => asyncLoad.isDone);

			if (currentlyRunningGame != null) {
				currentlyRunningGame.SetActive(false);
			}

			SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.ScenePath));
			_loadedScene = scene;

			//			transition.SetTrigger("End");
		}


		IEnumerator UnloadGame() {
			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
			yield return new WaitUntil(() => asyncLoad.isDone);
			_loadedScene = null;
			currentlyRunningGame.SetActive(true);
		}

		#endregion
	}
}