using System.Collections;
using GamehubPlugin.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamehubPlugin {
	public class GameHubManager : Singleton<GameHubManager> {
		private SceneReference _loadedScene;
		public GameObject currentlyRunningGame;


		#region Scene Loading

		public void LoadGameScene(SceneReference scene) {
			if (_loadedScene == null) {
				StartCoroutine(LoadGame(scene));
			}
		}

		public void UnloadScene() {
			if (currentlyRunningGame != null) {
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

			if (currentlyRunningGame != null) {
				currentlyRunningGame.SetActive(false);
			}

			SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.ScenePath));
			_loadedScene = scene;

			//			transition.SetTrigger("End");
		}


		IEnumerator UnloadGame() {
			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
			yield return new WaitUntil(() => asyncLoad.isDone);
			_loadedScene = null;
			currentlyRunningGame.SetActive(true);
		}

		#endregion
	}
}