﻿using System;
using System.Collections;
using System.Collections.Generic;
using DataStructures;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MotionAI.GameHub {
	public class GameHubManager : Singleton<GameHubManager> {
//		public Animator transition;


		private SceneReference _loadedScene;


		public GameObject uiCanvas;

		public void LoadGameScene(SceneReference scene) {
			if (_loadedScene == null) {
				StartCoroutine(LoadGame(scene));
			}
		}

		public void UnloadScene() {
			if (uiCanvas != null) {
				if (_loadedScene != null) {
					StartCoroutine(UnloadGame());
				}
			}
			else {
				Debug.Log("Would now return to the Gamehub");
			}
		}


		IEnumerator LoadGame(SceneReference scene) {
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
			yield return new WaitUntil(() => asyncLoad.isDone);

			if (uiCanvas != null) {
				uiCanvas.SetActive(false);
			}

			SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.ScenePath));
			_loadedScene = scene;

			//			transition.SetTrigger("End");
		}


		IEnumerator UnloadGame() {
			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
			yield return new WaitUntil(() => asyncLoad.isDone);
			_loadedScene = null;
			uiCanvas.SetActive(true);
		}
	}
}