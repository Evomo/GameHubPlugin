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
	[Serializable]
	public class HubSessionEvent : UnityEvent<CommunicationMessages> { }

	public class GameHubManager : Singleton<GameHubManager> {
		private Scene _loadedScene;


		[SerializeField] private Overlay overlayPrefab;
		private Overlay overlay;
		private MotionAIManager m_CurrentManager;
		private Session currSess;

		private List<Session> playSessionRuns;
		public UnityEvent onGameLoad, onGameQuit;
		public HubSessionEvent onSession;
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

		public void SendAllSessions() {
			// TODO send all

			CommunicationMessages msg = new CommunicationMessages();

			msg.sessions = playSessionRuns;
			msg.messageType = CommunicationMessageType.ENDGAME;

			onSession.Invoke(msg);
		}

		public void SendCurrentSession() {
			CommunicationMessages msg = new CommunicationMessages();

			msg.sessions = playSessionRuns;
			msg.messageType = CommunicationMessageType.CURRENTSESSION;
			onSession.Invoke(msg);
		}

		#endregion


		#region Wrapped methods

		public void LoadSceneWrapped(int scene) {
			if (_loadedScene.buildIndex <= 0) {
				StartCoroutine(LoadGame(scene));
			}
		}

		public void LoadSceneWrapped(GameHubGame game) {
			_settings = SessionSettings.CreateInstance(game);
			int sceneNum = BuildUtils.GetBuildScene(game.mainSceneReference.sceneAsset).buildIndex;
			LoadSceneWrapped(sceneNum);
		}

		private void UnloadScene() {
			if (_loadedScene.buildIndex > 0) {
				StartCoroutine(UnloadGame());
			}
		}


		private void QuitGame() {
			UnloadScene();
			SendAllSessions();
			onGameQuit.Invoke();
		}

		private void ResetScene() {
			if (_loadedScene.buildIndex > 0) {
				if (currSess != null) {
					playSessionRuns.Add(currSess);
				}

				EndSession();
				StartCoroutine(ResetGameCoroutine());
				StartSessionWrapper();
			}
		}

		private void StartSessionWrapper() {
			StartSessionWrapper(_settings.gameId, _settings.recordElmos);
		}

		private void StartSessionWrapper(int gameName, bool recordElmos) {
			if (currSess != null) {
				Debug.LogError("End current session before starting a new one");
				return;
			}

			try {
				m_CurrentManager = FindObjectOfType<MotionAIManager>();
				currSess = new Session(gameName, recordElmos);
				m_CurrentManager.controllerManager.onMovement.AddListener(currSess.RecordMovement);
			}
			catch (Exception) {
				Debug.LogError("No MotionAIManager present in scene ");
			}
		}

		private void EndSession(int score = 0, int coinsCollected = 0) {
			if (currSess != null) {
				currSess?.EndSession(score, coinsCollected);
				Debug.Log(currSess.ToString());
				SendCurrentSession();
			}

			currSess = null;
		}

		#endregion

		#region Public API

		public static void ResetGame() {
			GameHubManager.Instance.ResetScene();
		}


		public static void StartSession() {
			GameHubManager.Instance.StartSessionWrapper();
		}

		public static void StopGame() {
			GameHubManager.Instance.QuitGame();
		}

		#endregion
	}
}