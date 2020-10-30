using System;
using System.Collections;
using System.Collections.Generic;
using GamehubPlugin.Util;
using MotionAI.Core.Controller;
using MotionAI.Core.POCO;
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
        private SessionSettings m_Settings;


        public bool isGameRunning { get; private set; }

        #region Unity Lifecycle

        public void Awake() {
            m_Settings = new SessionSettings();
            playSessionRuns = new List<Session>();
            if (overlayPrefab != null) {
                overlay = Instantiate(overlayPrefab).GetComponent<Overlay>();
            }
        }

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
            isGameRunning = true;
        }


        IEnumerator UnloadGame() {
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            yield return new WaitUntil(() => asyncLoad.isDone);

            isGameRunning = false;
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

        private void SendAllSessions() {
            CommunicationMessages msg = new CommunicationMessages();

            msg.sessions = playSessionRuns;
            msg.messageType = CommunicationMessageType.ENDGAME;

            onSession.Invoke(msg);
        }

        private void SendCurrentSession(Session toSend) {
            if (currSess != null) {
                CommunicationMessages msg = new CommunicationMessages();

                msg.sessions = new List<Session> {toSend};
                msg.messageType = CommunicationMessageType.CURRENTSESSION;
                onSession.Invoke(msg);
            }
        }

        #endregion


        #region Wrapped methods

        public void LoadSceneWrapped(int scene) {
            if (_loadedScene.buildIndex <= 0) {
                StartCoroutine(LoadGame(scene));
            }
        }

        public void LoadSceneWrapped(GameHubGame game) {
            m_Settings = SessionSettings.CreateInstance(game);
            int sceneNum;

#if UNITY_EDITOR
            sceneNum = BuildUtils.GetBuildScene(game.mainSceneReference.sceneAsset).buildIndex;
#endif
#if !UNITY_EDITOR
			sceneNum = SceneManager.GetSceneByPath(game.mainSceneReference.ScenePath).buildIndex;

#endif

            LoadSceneWrapped(sceneNum);
        }

        private void UnloadScene() {
            playSessionRuns.Clear();
            if (_loadedScene.buildIndex > 0) {
                StartCoroutine(UnloadGame());
            }
        }


        private void QuitGame() {
            SendAllSessions();
            if (isGameRunning) {
                UnloadScene();
                onGameQuit.Invoke();
            }
            else {
                Debug.Log("Would now return to Gamehub ");
            }
        }

        private void ResetScene() {
            if (currSess != null) {
                Debug.LogError("Can't reset the scene without ending the session");
                return;
            }

            if (_loadedScene.buildIndex > 0) {
                StartCoroutine(ResetGameCoroutine());
                StartSessionWrapper();
            }
        }

        private void StartSessionWrapper() {
            if (m_Settings != null) {
                StartSessionWrapper(m_Settings.gameId, m_Settings.recordElmos);
            }
        }

        private void StartSessionWrapper(int gameName, bool recordElmos) {
            if (currSess != null) {
                Debug.LogError("End current session before starting a new one");
                return;
            }

            try {
                m_CurrentManager = FindObjectOfType<MotionAIManager>();
                if (overlay != null) {
                    overlay.UpdateManager(m_CurrentManager);
                }

                currSess = new Session(gameName, recordElmos);
                m_CurrentManager.controllerManager.onMovement.AddListener(SessionRecordCallback);
            }
            catch (Exception) {
                Debug.LogError("No MotionAIManager present in scene ");
            }
        }

        private void SessionRecordCallback(EvoMovement mov) {
            if (overlay != null) {
                if (!overlay.IsPaused) {
                    currSess.RecordMovement(mov);
                }
            }
        }

        private Session EndSessionWrapped() {
            Session toReturn = currSess;
            if (currSess != null) {
                toReturn = currSess?.EndSession();
                playSessionRuns.Add(toReturn);
                Debug.Log(currSess.ToString());
            }

            currSess = null;
            return toReturn;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Function to restart the game, the game scene is reloaded and can only be used when a session has been ended
        /// </summary>
        public static void ResetGame() {
            GameHubManager.Instance.ResetScene();
        }


        /// <summary>
        /// Function that tells the manager to start recording movements
        /// </summary>
        public static void StartSession() {
            GameHubManager.Instance.StartSessionWrapper();
        }

        /// <summary>
        /// Ends the session with the given parameters and stores them in a list for all the sessions played in a continuous use of the gamehub
        /// </summary>
        public static void EndSession() {
            Session toSend = GameHubManager.Instance.EndSessionWrapped();
            GameHubManager.Instance.SendCurrentSession(toSend);
        }

        /// <summary>
        /// Stops the game and returns to the main app
        /// </summary>
        public static void StopGame() {
            GameHubManager.Instance.QuitGame();
        }

        /// <summary>
        /// Updates the coins for the current session
        /// </summary>
        /// <param name="coins"></param>
        public static void SetCoins(int coins) {
            Session curr = GameHubManager.Instance.currSess;
            if (curr != null) {
                curr.coinsCollected = coins;
            }
        }

        /// <summary>
        /// Updates the score for the current session
        /// </summary>
        /// <param name="score"></param>
        public static void SetScore(int score) {
            Session curr = GameHubManager.Instance.currSess;
            if (curr != null) {
                curr.score = score;
            }
        }

        /// <summary>
        /// Updates the lives displayed in the overlay
        /// </summary>
        /// <param name="lives"></param>
        public static void SetLives(int currentLives) {
//TODO implement
            return;
        }

        #endregion
    }
}