using System;
using GamehubPlugin.Core.OverlaySpace;
using MotionAI.Core.Controller;
using MotionAI.Core.Util;
using UnityEngine;
using UnityEngine.Events;

namespace GamehubPlugin.Core {
    #region Extra classes

    [Serializable]
    public class OverlayEvents {
        public UnityEvent onPause, onQuit, onResume;
    }

    #endregion

    public class Overlay : Singleton<Overlay> {
        [SerializeField] private OverlayPanel vertical, horizontal;
        [SerializeField] public OverlayEvents events;
        [SerializeField] private bool _isPaused;

        private MotionAIManager m_Manager;
        
        public OverlayPanel activePanel;

        private bool _isPanelActive;

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

                    if (_isPanelActive) {
                        activePanel.gameObject.SetActive(!_isPaused);
                    }
                }
            }
        }

        private void ResetPanel(OverlayPanel p, ScorePanelColorScheme cs) {
            p.coins.text = "0";
            p.lives.text = "0";
            p.score.text = "0";
            p.panelBackground.color = cs.backgroundColor;
            p.scoreIcon.color = cs.scoreColor;
            p.pauseIcon.color = cs.backgroundColor;
            p.gameObject.SetActive(false);
        }

        public void ResetPanel(GameHubGame game) {
            ScorePanelColorScheme cs = game.overlayOptions.colorScheme;

            ResetPanel(vertical, cs);
            ResetPanel(horizontal, cs);

            _isPanelActive = game.overlayOptions.useOverlay;
            activePanel = game.overlayOptions.overlayType == OverlayType.Horizontal ? horizontal : vertical;
            
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