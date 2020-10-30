using System;
using MotionAI.Core.Controller;
using MotionAI.Core.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GamehubPlugin.Core {
    [Serializable]
    public class OverlayObjects {
        public Image panelBackground, scoreIcon, pauseIcon;
        public TextMeshProUGUI coins, lives, score;
        public Button pause;
        public GameObject gameScorePanel;
    }
    [Serializable]
    public class OverlayEvents {
        public UnityEvent onPause, onQuit;
        
    } 
    public class Overlay : Singleton<Overlay> {
        private MotionAIManager m_Manager;
        [SerializeField] public OverlayObjects components;
        [SerializeField] public OverlayEvents events;
        [SerializeField] private bool _isPaused;

        public bool IsPaused {
            get { return _isPaused; }
            set {
                if (_isPaused != value) {
                    if (_isPaused) {
                        Pause();
                    }
                    else {
                        Resume();
                    }
                }
            }
        }


        public void Start() {
            GameHubManager.Instance.hubEvents.onGameLoad.AddListener(Pause);
        }


        public void ResetPanel(GameHubGame game) {
            components.coins.text = "0";
            components.lives.text = "0";
            components.score.text = "0";
            components.panelBackground.color = game.colors.backgroundColor;
            components.scoreIcon.color = game.colors.scoreColor;
            components.pauseIcon.color = game.colors.backgroundColor;
            IsPaused = true;
            Pause();
        }

        public void UpdateManager(MotionAIManager manager) {
            m_Manager = manager;
        }

        public void TogglePause() {
            IsPaused = !IsPaused;
            if (m_Manager != null) {
                m_Manager.isTracking = !IsPaused;
                if (IsPaused) {
                    events.onPause.Invoke();

                }
            }
        }

        public void QuitGame() {
            Resume();
            GameHubManager.StopGame();
        }

        public void Resume() {
            _isPaused = true;
            Time.timeScale = 1f;
            //IsPaused = false;
            components.gameScorePanel.SetActive(true);
        }

        public void Pause() {
            _isPaused = false;
            Time.timeScale = 0;
            //	IsPaused = true;

            components.gameScorePanel.SetActive(false);
        }
    }
}