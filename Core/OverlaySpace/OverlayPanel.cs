using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamehubPlugin.Core.OverlaySpace {
    public class OverlayPanel : MonoBehaviour {
        public Image panelBackground, scoreIcon, pauseIcon;
        public TextMeshProUGUI coins, lives, score;
        public Button pause;
        public GameObject gameScorePanel;
    }
    
}