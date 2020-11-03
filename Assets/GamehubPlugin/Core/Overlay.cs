using System;
using MotionAI.Core.Controller;
using MotionAI.Core.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GamehubPlugin.Core {
    #region Extra classes

    [Serializable]
    public class OverlayObjects {
        public Image panelBackground, scoreIcon, pauseIcon;
        public TextMeshProUGUI coins, lives, score;
        public Button pause;
        public GameObject gameScorePanel;
    }

    [Serializable]
    public class OverlayEvents {
        public UnityEvent onPause, onQuit, onResume;
    }

    #endregion

    public class Overlay : Singleton<Overlay> {
        private MotionAIManager m_Manager;
        [SerializeField] public OverlayObjects components;
        [SerializeField] public OverlayEvents events;
        [SerializeField] private bool _isPaused;

        public bool IsPaused {
            get { return _isPaused; }
            set {
                if (_isPaused != value) {
                    if (!_isPaused) {
                        Pause();
                    }
                    else {
                        Resume();
                    }

                    components.gameScorePanel.SetActive(!_isPaused);
                }
            }
        }


        public void ResetPanel(GameHubGame game) {
            components.coins.text = "0";
            components.lives.text = "0";
            components.score.text = "0";
            components.panelBackground.color = game.colors.backgroundColor;
            components.scoreIcon.color = game.colors.scoreColor;
            components.pauseIcon.color = game.colors.backgroundColor;
            IsPaused = true;
        }

        public void UpdateManager(MotionAIManager manager) {
            m_Manager = manager;
        }

        public void TogglePause() {
            IsPaused = !IsPaused;
            if (m_Manager != null) {
                m_Manager.isTracking = !IsPaused;
            }
        }

        public void QuitGame() {
            Resume();
            events.onQuit.Invoke();
        }

        public void Resume() {
            _isPaused = false;
            Time.timeScale = 1f;
            events.onResume.Invoke();
        }

        public void Pause() {
            _isPaused = true;
            Time.timeScale = 0;
            events.onPause.Invoke();
        }
    }
}