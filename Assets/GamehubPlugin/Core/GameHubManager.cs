using System;
using System.Collections;
using MotionAI.Core.Controller;
using MotionAI.Core.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GamehubPlugin.Core {
	public class GameHubManager : Singleton<GameHubManager> {
		private Scene _loadedScene;


		[SerializeField] private Overlay overlayPrefab;
		private Overlay overlay;
		private MotionAIManager m_CurrentManager;
		private Session currSess;

		public UnityEvent OnEndSession, OnStartSession, onGameLoad;

		#region Unity Lifecycle

		public void Awake() {
			overlay = Instantiate(overlayPrefab).GetComponent<Overlay>();
		}

		#endregion

		#region Session Handle

		#endregion

		#region Scene Loading

		IEnumerator LoadGame(int sceneBuildNumber) {
			Scene scene = SceneManager.GetSceneByBuildIndex(sceneBuildNumber);
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneBuildNumber, LoadSceneMode.Additive);
			yield return new WaitUntil(() => asyncLoad.isDone);


			_loadedScene = SceneManager.GetSceneByBuildIndex(sceneBuildNumber);
			SceneManager.SetActiveScene(_loadedScene);
			onGameLoad.Invoke();
		}


		IEnumerator UnloadGame() {
			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
			yield return new WaitUntil(() => asyncLoad.isDone);
			_loadedScene = SceneManager.GetSceneByBuildIndex(0);
		}

		#endregion

		#region Public API

		public void LoadGameScene(int scene) {
			if (_loadedScene.buildIndex <= 0) {
				StartCoroutine(LoadGame(scene));
			}
		}

		public void UnloadScene() {
			if (_loadedScene.buildIndex > 0) {
				StartCoroutine(UnloadGame());
			}
			else {
				Debug.Log("Would now return to the Gamehub");
			}
		}

		public static void LoadScene(int sceneNumber) {
			GameHubManager.Instance.LoadGameScene(sceneNumber);
		}


		public void StartSession(string gameName) {
			m_CurrentManager = FindObjectOfType<MotionAIManager>();
			currSess = new Session(gameName);
			m_CurrentManager.controllerManager.onMovement.AddListener(currSess.RecordMovement);
		}

		public void EndSession(int score = 0, int coinsCollected = 0) {
			if (currSess != null) {
				currSess?.EndSession(score, coinsCollected);
				Debug.Log(currSess.ToString());
				//TODO send to native sdk 
			}

			currSess = null;
		}

		public static void StopGame() {
			GameHubManager.Instance.UnloadScene();
		}

		#endregion
	}
}