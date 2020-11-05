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

    public class GameHubManager : Singleton<GameHubManager> {
        private Scene _loadedScene;


        private bool _hasNotifiedApp;
        [SerializeField] private Overlay overlayPrefab;
        private Overlay overlay;

        [SerializeField] private MotionAIManager m_CurrentManager;
        private Session currSess;

        public GameHubGame loadedGame;

        public bool isGameRunning { get; private set; }

        #region Unity Lifecycle

        public void Awake() {
            if (overlayPrefab != null) {
                overlay = Instantiate(overlayPrefab).GetComponent<Overlay>();
                overlay.events.onPause.AddListener(() => SendCurrentSession(currSess));
            }

            loadedGame = null;
        }

        #endregion


        #region Scene Loading

        IEnumerator LoadGame(int sceneBuildNumber, GameHubGame game) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneBuildNumber, LoadSceneMode.Additive);
            yield return new WaitUntil(() => asyncLoad.isDone);

            _loadedScene = SceneManager.GetSceneByBuildIndex(sceneBuildNumber);
            SceneManager.SetActiveScene(_loadedScene);
            overlay.ResetPanel(game);
            loadedGame = game;
            isGameRunning = true;
        }


        IEnumerator UnloadGame() {
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            yield return new WaitUntil(() => asyncLoad.isDone);

            isGameRunning = false;
            _hasNotifiedApp = false;
            loadedGame = null;
            m_CurrentManager = null;

            _loadedScene = SceneManager.GetSceneByBuildIndex(0);
        }

        IEnumerator ResetGameCoroutine() {
            int sceneNumber = _loadedScene.buildIndex;
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneNumber);
            yield return new WaitUntil(() => asyncUnload.isDone);
            StartCoroutine(LoadGame(sceneNumber, loadedGame));
        }

        #endregion

        #region App communication

        private void SendCurrentSession(Session toSend, bool gameOver = false) {
            if (currSess != null) {
                CommunicationMessages msg = new CommunicationMessages();

                msg.session = toSend;
                msg.messageType = gameOver ? CommunicationMessageType.GAMEOVER : CommunicationMessageType.PAUSE;
                if (m_CurrentManager != null) {
                    m_CurrentManager.SendGameHubMessage(msg.ToString());
                }
            }
        }

        #endregion


        #region Wrapped methods

        public void LoadSceneWrapped(GameHubGame game) {
            int sceneNum;

#if UNITY_EDITOR
            sceneNum = BuildUtils.GetBuildScene(game.mainSceneReference.sceneAsset).buildIndex;
#endif
#if !UNITY_EDITOR
			sceneNum = SceneManager.GetSceneByPath(game.mainSceneReference.ScenePath).buildIndex;

#endif

            if (_loadedScene.buildIndex <= 0) {
                StartCoroutine(LoadGame(sceneNum, game));
                StartSessionWrapper();
            }
        }


        private void QuitGame() {
            if (isGameRunning) {
                if (_loadedScene.buildIndex > 0) {
                    StartCoroutine(UnloadGame());
                }
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
                SendCurrentSession(currSess, true);

                StartCoroutine(ResetGameCoroutine());
                StartSessionWrapper();
            }
        }

        private void StartSessionWrapper() {
            if (loadedGame != null) {
                if (currSess != null) {
                    Debug.LogError("End current session before starting a new one");
                    return;
                }

                try {
                    m_CurrentManager = FindObjectOfType<MotionAIManager>();

                    if (!_hasNotifiedApp) {
                        CommunicationMessages cm = new CommunicationMessages();
                        cm.messageType = CommunicationMessageType.GAMELOADED;
                        m_CurrentManager.SendGameHubMessage(cm.ToString());
                        _hasNotifiedApp = true;
                    }

                    if (overlay != null) {
                        overlay.UpdateManager(m_CurrentManager);
                    }

                    currSess = new Session(loadedGame);
                    m_CurrentManager.controllerManager.onMovement.AddListener(SessionRecordCallback);
                }
                catch (Exception) {
                    Debug.LogError("No MotionAIManager present in scene ");
                }
            }
        }


        private void SessionRecordCallback(EvoMovement mov) {
            if (overlay != null) {
                if (!overlay.IsPaused) {
                    currSess.RecordMovement(mov);
                }
            }
        }

        private Session EndSessionWrapped(int score, int coinsCollected) {
            if (currSess != null) {
                currSess.score = score;
                currSess.coinsCollected = coinsCollected;
            }

            return EndSessionWrapped();
        }

        private Session EndSessionWrapped() {
            Session toReturn = currSess;
            if (currSess != null) {
                toReturn = currSess?.EndSession();
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


        public static void EndSession(int score, int coinsCollected) {
            Session toSend = GameHubManager.Instance.EndSessionWrapped(score, coinsCollected);
            GameHubManager.Instance.SendCurrentSession(toSend);
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
            Overlay o = GameHubManager.Instance.overlay;
            if (o != null) o.QuitGame();
        }

        /// <summary>
        /// Updates the coins for the current session
        /// </summary>
        /// <param name="coins"></param>
        public static void SetCoins(int coins) {
            Session curr = GameHubManager.Instance.currSess;
            if (curr != null) {
                curr.coinsCollected = coins;
                GameHubManager.Instance.overlay.components.coins.text = $"{coins}";
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
                GameHubManager.Instance.overlay.components.score.text = $"{score}";
            }
        }

        /// <summary>
        /// Updates the lives displayed in the overlay
        /// </summary>
        /// <param name="lives"></param>
        public static void SetLives(int currentLives) {
            Overlay o = GameHubManager.Instance.overlay;
            if (o != null) o.components.coins.text = $"{currentLives}";
        }


        public static void Pause() {
            Overlay o = GameHubManager.Instance.overlay;
            if (o != null) {
                o.Pause();
            }
            else {
                Debug.LogWarning("No Overlay, but would pause now");
            }
        }

        public static void Resume() {
            Overlay o = GameHubManager.Instance.overlay;
            if (o != null) {
                o.Resume();
            }
            else {
                Debug.LogWarning("No Overlay, but would resume game now ");
            }
        }

        public static void Quit() {
            Overlay o = GameHubManager.Instance.overlay;
            if (o != null) {
                o.QuitGame();
            }
            else {
                Debug.LogWarning("No Overlay, but would quit game ");
            }
        }

        #endregion
    }
}