using System;
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

        public OverlayPanel activePanel;

        private bool _isPanelActive;

        public bool IsPaused {
            get { return _isPaused; }
            set {
                if (_isPaused != value) {

                    _isPaused = value;
                    activePanel.gameObject.SetActive(!_isPaused && _isPanelActive);
                }
            }
        }

        private void Start() {
            activePanel = vertical;
        }

        private void ResetPanel(OverlayPanel p, GameHubGame game) {
            ScorePanelColorScheme cs = game.overlayOptions.colorScheme;
            PanelOptions po = game.overlayOptions.usedPanels;
            Color textCol = game.overlayOptions.colorScheme.textColor;

            p.coins.text = "0";
            p.lives.text = "0";
            p.score.text = "0";
            p.coins.color = textCol;
            p.lives.color = textCol;
            p.score.color = textCol;
            p.panelBackground.color = cs.backgroundColor;
            p.scoreIcon.color = cs.scoreColor;
            p.pauseIcon.color = cs.backgroundColor;

            p.coinPanel.SetActive(po.HasFlag(PanelOptions.COINS));
            p.livePanel.SetActive(po.HasFlag(PanelOptions.LIVES));
            p.gameObject.SetActive(false);
        }

        public void ResetPanel(GameHubGame game) {
            ResetPanel(vertical, game);
            ResetPanel(horizontal, game);

            activePanel = game.overlayOptions.menuType == DeviceOrientation.Horizontal ? horizontal : vertical;
            _isPanelActive = game.overlayOptions.usedPanels != 0;
            activePanel.gameObject.SetActive(true);
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

        public void TogglePause() {
            if (IsPaused) {
                events.onResume.Invoke();
            }
            else {
                events.onPause.Invoke();
            }

        }
        public void Resume() {
            AudioListener.pause = false;
            IsPaused = false;
            Time.timeScale = 1f;
        }

        public void Pause() {
            AudioListener.pause = true;
            IsPaused = true;
            Time.timeScale = 0;
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
