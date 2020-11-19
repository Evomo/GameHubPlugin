﻿using System;
using Cysharp.Text;
using GamehubPlugin.Core.OverlaySpace;
using MotionAI.Core.Controller;
using MotionAI.Core.Util;
using UnityEngine;
using UnityEngine.Events;

namespace GamehubPlugin.Core {
    #region Extra classes

    [Serializable]
    public class OverlayEvents {
        public UnityEvent onPause, onResume;
    }


    public enum OverlayPanelEnum {
        LIVES,
        SCORE,
        COINS
    }

    #endregion

    [RequireComponent(typeof(GameHubManager))]
    public class Overlay : MonoBehaviour {
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

        private void Start() {
            activePanel = vertical;
        }

        private void ResetPanel(OverlayPanel p, ScorePanelColorScheme cs,PanelOptions po) {
            p.coins.text = "0";
            p.lives.text = "0";
            p.score.text = "0";
            p.coins.color = cs.overLayTextColor;
            p.lives.color = cs.overLayTextColor;
            p.score.color = cs.overLayTextColor;
            p.panelBackground.color = cs.backgroundColor;
            p.scoreIcon.color = cs.scoreColor;
            p.pauseIcon.color = cs.backgroundColor;
            p.coinPanel.SetActive(po.useCoin);
            p.livePanel.SetActive(po.useLive);
            p.gameObject.SetActive(false);
        }

        public void ResetPanel(GameHubGame game) {
            ScorePanelColorScheme cs = game.overlayOptions.colorScheme;
            PanelOptions po = game.overlayOptions.panelOptions;
            ResetPanel(vertical, cs,po);
            ResetPanel(horizontal, cs,po);

            _isPanelActive = game.overlayOptions.useOverlay;
            activePanel = game.overlayOptions.menuType == DeviceOrientation.Horizontal ? horizontal : vertical;


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


        public void UpdatePanelValue<T>(OverlayPanelEnum toChange, T value) {
            if (activePanel != null) {
                using (var sb = ZString.CreateStringBuilder()) {
                    sb.Append(value);
                    switch (toChange) {
                        case OverlayPanelEnum.COINS:
                            activePanel.coins.SetText(sb);
                            break;
                        case OverlayPanelEnum.LIVES:
                            activePanel.lives.SetText(sb);
                            break;
                        case OverlayPanelEnum.SCORE:
                            activePanel.score.SetText(sb);
                            break;
                    }
                }
            }
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

        public void OnApplicationPause(bool pauseStatus) {
            if (pauseStatus) {
                Pause();
            }
            else {
                Resume();
            }
        }
    }
    
    
}