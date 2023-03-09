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
          // name it for easy recognition
          _instance.name = _instance.GetType().ToString();
          // mark root as DontDestroyOnLoad();
          DontDestroyOnLoad(_instance.gameObject);
        }
        return _instance;
      }
    }
    
    public PreloadedScene PreloadScene(Scenes.Name sceneName) {
      var asyncOperation = SceneManager.LoadSceneAsync(sceneName.SceneName());
      asyncOperation.allowSceneActivation = false;
      // Fix this, cannot run coroutines in this context
      StartCoroutine(LoadRoutine(asyncOperation));
      return new PreloadedScene(asyncOperation);
    }

    private IEnumerator LoadRoutine(AsyncOperation asyncOperation) {
      while (!asyncOperation.isDone) {
        yield return null;
      }
    }
  }
}