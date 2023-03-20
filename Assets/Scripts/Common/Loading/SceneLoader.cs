using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Loading {
  public class SceneLoader : MonoBehaviour {
    private static SceneLoader _instance;
    public static SceneLoader Instance {
      get {
        if (!_instance) {
          _instance = new GameObject("SceneLoader").AddComponent<SceneLoader>();
          DontDestroyOnLoad(_instance.gameObject);
        }
        return _instance;
      }
    }

    private Coroutine _currentLoadRoutine;
    
    public PreloadedScene PreloadScene(Scenes.Name sceneName) {
      var scene = new PreloadedScene();
      _currentLoadRoutine = StartCoroutine(LoadRoutine(sceneName, scene));
      return scene;
    }

    private IEnumerator LoadRoutine(Scenes.Name sceneName, PreloadedScene preloadedScene) {
      yield return _currentLoadRoutine;

      var asyncOperation = SceneManager.LoadSceneAsync(sceneName.SceneName());
      asyncOperation.allowSceneActivation = false;
      while (!asyncOperation.isDone) {
        if (preloadedScene.IsActivated()) {
          asyncOperation.allowSceneActivation = true;
        }
        yield return null;
      }
    }
  }
}