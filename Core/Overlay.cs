using System;
using MotionAI.Core.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GamehubPlugin.Core {
	public class Overlay : Singleton<Overlay> {
		public Button start, settings, quit, tutorial;
		public GameObject pauseMenuUI;


		public UnityEvent onSettings, onTutorial;
		public static bool isPaused; 

		
		public void Start() {
			GameHubManager.Instance.onGameLoad.AddListener(Pause);
			
			
			tutorial.onClick.AddListener(() => onTutorial.Invoke());
			settings.onClick.AddListener(() => onSettings.Invoke());
			start.onClick.AddListener(Resume);
		}

		private void QuitGame() {
			Resume();
			GameHubManager.StopGame();
		}
		private void Resume() {
			Time.timeScale = 1f;
			isPaused = false;
			pauseMenuUI.SetActive(false);
		}

		private void Pause() {
			Time.timeScale = 0;
			isPaused = true;
			pauseMenuUI.SetActive(true);
		}
		
	}
}