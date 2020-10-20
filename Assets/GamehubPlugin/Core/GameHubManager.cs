using System;
using System.Collections;
using System.Collections.Generic;
using GamehubPlugin.Util;
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

		private List<Session> playSessionRuns;
		public UnityEvent OnEndSession, OnStartSession, onGameLoad;

		private SessionSettings _settings;

		#region Unity Lifecycle

		public void Awake() {
			_settings = new SessionSettings();
			playSessionRuns = new List<Session>();
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

			playSessionRuns.Clear();
			_loadedScene = SceneManager.GetSceneByBuildIndex(sceneBuildNumber);
			SceneManager.SetActiveScene(_loadedScene);
			onGameLoad.Invoke();
		}


		IEnumerator UnloadGame() {
			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
			yield return new WaitUntil(() => asyncLoad.isDone);

			_loadedScene = SceneManager.GetSceneByBuildIndex(0);
		}

		IEnumerator ResetGameCoroutine() {
			int sceneNumber = _loadedScene.buildIndex;
			AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneNumber);
			yield return new WaitUntil(() => asyncUnload.isDone);
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber, LoadSceneMode.Additive);
			yield return new WaitUntil(() => asyncLoad.isDone);
			_loadedScene = SceneManager.GetSceneByBuildIndex(sceneNumber);


			SceneManager.SetActiveScene(_loadedScene);
		}

		#endregion

		#region App communication

		public void SendAllSession() {
			// TODO send all

			CommunicationMessages msg = new CommunicationMessages();

			msg.sessions = playSessionRuns;
			msg.messageType = CommunicationMessageType.ENDGAME;

			SendMessageToApp(JsonUtility.ToJson(msg));
		}

		public void SendCurrentSession() {
			CommunicationMessages msg = new CommunicationMessages();

			msg.sessions = playSessionRuns;
			msg.messageType = CommunicationMessageType.CURRENTSESSION;
			SendMessageToApp(JsonUtility.ToJson(msg));
		}

		public void SendMessageToApp(string message) {
			//TODO send to app
		}

		#endregion

		#region Public API

		#region Wrapped methods

		private void LoadSceneWrapped(int scene) {
			if (_loadedScene.buildIndex <= 0) {
				StartCoroutine(LoadGame(scene));
			}
		}

		private void LoadSceneWrapped(GameHubGame game) {
			_settings = SessionSettings.CreateInstance(game);
			LoadSceneWrapped(SceneManager.GetSceneByPath(game.mainSceneReference.ScenePath).buildIndex);
		}

		private void UnloadScene() {
			if (_loadedScene.buildIndex > 0) {
				StartCoroutine(UnloadGame());
			}
		}


		public static void LoadScene(GameHubGame game) {
			GameHubManager.Instance.LoadSceneWrapped(game);
		}

		public static void LoadScene(int sceneNumber) {
			GameHubManager.Instance.LoadSceneWrapped(sceneNumber);
		}
		
		private void ResetScene() {
			if (_loadedScene.buildIndex > 0) {
				if (currSess != null) {
					playSessionRuns.Add(currSess);
				}

				StartCoroutine(ResetGameCoroutine());
				StartSessionWrapper();
			}
		}

		private void StartSessionWrapper() {
			StartSessionWrapper(_settings.gameId, _settings.recordElmos);
		}

		private void StartSessionWrapper(int gameName, bool recordElmos) {
			try {
				m_CurrentManager = FindObjectOfType<MotionAIManager>();
				currSess = new Session(gameName, recordElmos);
				m_CurrentManager.controllerManager.onMovement.AddListener(currSess.RecordMovement);
			}
			catch (Exception e) {
				Debug.LogError("No MotionAIManager present in scene ");
			}
		}

		#endregion


		public void EndSession(int score = 0, int coinsCollected = 0) {
			if (currSess != null) {
				currSess?.EndSession(score, coinsCollected);
				Debug.Log(currSess.ToString());
				//TODO send to native sdk 
			}

			currSess = null;
		}


		public static void ResetGame() {
			GameHubManager.Instance.ResetScene();
		}


		public static void StopGame() {
			GameHubManager.Instance.UnloadScene();
		}

		#endregion
	}
}