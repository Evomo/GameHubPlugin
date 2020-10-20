using System;
using System.Linq;
using MotionAI.Core.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GamehubPlugin.Core {
	public class Overlay : Singleton<Overlay> {
		public Button start, pause, quit, tutorial;
		public GameObject pauseMenuUI;


		public UnityEvent onSettings, onTutorial;

		[SerializeField] private bool isPaused;

		public bool IsPaused {
			get { return isPaused; }
			set {
				isPaused = value;
				if (isPaused) {
					Pause();
				}
				else {
					Resume();
				}
			}
		}


		public void Start() {
			GameHubManager.Instance.onGameLoad.AddListener(Pause);
		}


		public void TogglePause() {
			IsPaused = !IsPaused;
		}

		public void QuitGame() {
			Resume();
			GameHubManager.StopGame();
		}

		public void Resume() {
			Time.timeScale = 1f;
			//IsPaused = false;
			pauseMenuUI.SetActive(false);
		}

		public void Pause() {
			Time.timeScale = 0;
		//	IsPaused = true;
			pauseMenuUI.SetActive(true);
		}
	}
}