using System;
using System.Collections;
using System.Collections.Generic;
using GamehubPlugin.Core.Util;
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
        private bool _hasNotifiedApp;

        [SerializeField] private Overlay _overlay;
        
        [NonSerialized]private Session _currSess;
        private Scene _loadedScene;

        [SerializeField] private Overlay overlayPrefab;
        [SerializeField] private MotionAIManager m_CurrentManager;


        public GameHubGame loadedGame;

        public bool isGameRunning { get; private set; }
        public Overlay Overlay() { return _overlay; }

        #region Unity Lifecycle

        public void Awake() {
            _overlay = GetComponentInChildren<Overlay>();
            _currSess = null;
            if (_overlay == null) {
                if (overlayPrefab == null) {
                    overlayPrefab = Resources.Load<Overlay>("Overlay");
                    _overlay = Instantiate(overlayPrefab).GetComponent<Overlay>();
                    _overlay.transform.SetParent(transform);
                }
            }

                
            
            _overlay.events.onPause.AddListener((() => HandlePause(true)));
            _overlay.events.onResume.AddListener((() => HandlePause(false)));


            loadedGame = null;
        }

        #endregion


        #region Scene Loading

        IEnumerator LoadGame(int sceneBuildNumber, GameHubGame game) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneBuildNumber, LoadSceneMode.Additive);
            yield return new WaitUntil(() => asyncLoad.isDone);

            _loadedScene = SceneManager.GetSceneByBuildIndex(sceneBuildNumber);
            SceneManager.SetActiveScene(_loadedScene);
            _overlay.ResetPanel(game);
            loadedGame = game;
            isGameRunning = true;
            StartSessionWrapper();
        }


        private void CleanMainScene() {
            DontDestroyOnLoadManager.DestroyAll();

            foreach (GameObject o in SceneManager.GetSceneByBuildIndex(0).GetRootGameObjects()) {
                if (!o.activeInHierarchy) {
                    Destroy(o);
                    Debug.Log($"Deleting {o.name}");
                }
            }
        }

        IEnumerator UnloadGame() {
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            yield return new WaitUntil(() => asyncLoad.isDone);

            isGameRunning = false;
            _hasNotifiedApp = false;
            loadedGame = null;
            m_CurrentManager = null;

            _loadedScene = SceneManager.GetSceneByBuildIndex(0);
            Application.Unload();

        }

        IEnumerator ResetGameCoroutine() {
            int sceneNumber = _loadedScene.buildIndex;
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneNumber);
            yield return new WaitUntil(() => asyncUnload.isDone);
            yield return StartCoroutine(LoadGame(sceneNumber, loadedGame));
            _overlay.ResetPanel(loadedGame);

            _overlay.events.onResume.Invoke();
        }

        #endregion

        #region App communication

        private void SendCurrentSession(bool gameOver = false) {
            if (_currSess != null) {
                CommunicationMessages msg = new CommunicationMessages();

                msg.session = _currSess.EndSession();
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
            if (sceneNum < 0) {
                Debug.LogError($"Scene for {game.gameName}must be added to build to load from the Gamehub");
                return;
            }
#endif
#if !UNITY_EDITOR
			sceneNum = SceneUtility.GetBuildIndexByScenePath(game.mainSceneReference.ScenePath);

#endif

            if (_loadedScene.buildIndex <= 0) {
                StartCoroutine(LoadGame(sceneNum, game));
                _overlay.events.onPause.Invoke();
            }
        }

        public static string GetSceneNameFromBuildIndex(int index) {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(index);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            return sceneName;
        }

        private void QuitGame() {
            if (isGameRunning) {
                if (_loadedScene.buildIndex > 0) {
                    _currSess = null;
                    CleanMainScene();
                    StartCoroutine(UnloadGame());
                }
            }
            else {
                Debug.Log("Would now return to Gamehub ");
            }
        }

        private void ResetScene() {
            if (_loadedScene.buildIndex > 0) {
                EndSessionWrapped();
                StartCoroutine(ResetGameCoroutine());
                CleanMainScene();
            }
        }

        private void StartSessionWrapper() {
            if (loadedGame != null) {
                if (_currSess != null) {
                    Debug.LogError("End current session before starting a new one");
                    return;
                }

                m_CurrentManager = FindObjectOfType<MotionAIManager>();
                if (m_CurrentManager != null) {
                    _currSess = new Session(loadedGame);

                    if (!_hasNotifiedApp) {
                        CommunicationMessages cm = new CommunicationMessages();
                        cm.session = _currSess;
                        cm.messageType = CommunicationMessageType.GAMELOADED;
                        m_CurrentManager.SendGameHubMessage(cm.ToString());
                        _hasNotifiedApp = true;
                    }

                    if (_overlay != null) {
                        _overlay.UpdateManager(m_CurrentManager);
                    }

                    m_CurrentManager.controllerManager.onMovement.AddListener(SessionRecordCallback);
                }
            }
            else {
                Debug.LogError("No MotionAIManager present in scene ");
            }
        }


        private void SessionRecordCallback(EvoMovement mov) {
            if (_overlay != null) {
                if (!_overlay.IsPaused) {
                    _currSess.RecordMovement(mov);
                }
            }
        }

        private void EndSessionWrapped(int score, int coinsCollected) {
            if (_currSess != null) {
                _currSess.score = score;
                _currSess.coinsCollected = coinsCollected;
            }

            EndSessionWrapped();
        }

        private void EndSessionWrapped() {
            if (_currSess != null) {
                SendCurrentSession(gameOver: true);
            }

            _currSess = null;
        }


        public void HandlePause(bool shouldPause) {
            if (_overlay != null) {
                _currSess?.TogglePause(shouldPause);
                if (shouldPause) {
                    _overlay.Pause();
                    SendCurrentSession();
                }
                else {
                    _overlay.Resume();
                }
            }

        }

        #endregion

        #region Public API

        /// <summary>
        /// Function to restart the game, the game scene is reloaded and can only be used when a session has been ended
        /// </summary>
        public static void ResetGame() {
            GameHubManager.Instance.ResetScene();
        }


        public static void EndSession(int score, int coinsCollected) {
            GameHubManager.Instance.EndSessionWrapped(score, coinsCollected);
        }


        /// <summary>
        /// Ends the session with the given parameters and stores them in a list for all the sessions played in a continuous use of the gamehub
        /// </summary>
        public static void EndSession() {
            GameHubManager.Instance.EndSessionWrapped();
        }

        /// <summary>
        /// Stops the game and returns to the main app
        /// </summary>
        public static void Quit() {
            GameHubManager.Instance.QuitGame();
        }

        /// <summary>
        /// Updates the coins for the current session
        /// </summary>
        /// <param name="coins"></param>
        public static void SetCoins(int coins) {
            Session curr = GameHubManager.Instance._currSess;
            if (curr != null) {
                curr.coinsCollected = coins;
                Overlay o = GameHubManager.Instance._overlay;
                if (o != null) o.UpdatePanelValue(OverlayPanelEnum.COINS, coins);
            }
        }

        /// <summary>
        /// Updates the score for the current session
        /// </summary>
        /// <param name="score"></param>
        public static void SetScore(int score) {
            Session curr = GameHubManager.Instance._currSess;
            if (curr != null) {
                curr.score = score;
                Overlay o = GameHubManager.Instance._overlay;
                if (o != null) o.UpdatePanelValue(OverlayPanelEnum.SCORE, score);
            }
        }

        /// <summary>
        /// Updates the lives displayed in the overlay
        /// </summary>
        /// <param name="lives"></param>
        public static void SetLives(int currentLives) {
            Overlay o = GameHubManager.Instance._overlay;
            if (o != null) o.UpdatePanelValue(OverlayPanelEnum.LIVES, currentLives);
        }


        /// <summary>
        /// Pauses the game and sends a message to the gamehub main app 
        /// </summary> 
        public static void Pause() {
            GameHubManager.Instance.HandlePause(true);
        }


        /// <summary>
        /// Resumes the game and sets the time scale back to normal 
        /// </summary>
        public static void Resume() {
            GameHubManager.Instance.HandlePause(false);
        }

        #endregion
    }
}