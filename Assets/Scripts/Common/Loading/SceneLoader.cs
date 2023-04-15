using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Common.Loading {
  public class SceneLoader : MonoBehaviour {
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float fadeInDuration = 0.25f;
    private VisualElement _fade;
    public static SceneLoader Instance { get; private set; }

    private Coroutine _currentLoadRoutine;
    
    private void Awake() {
      if (!Instance) {
        Instance = this;
      } if (Instance != this) {
        Destroy(gameObject);
      }
    }

    private void Start() {
      _fade = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ScreenFade");
    }

    public void LoadScene(SceneId sceneName) {
      StartCoroutine(SynchronousLoadRoutine(sceneName));
    }
    
    public PreloadedScene PreloadScene(SceneId sceneName) {
      var scene = new PreloadedScene();
      _currentLoadRoutine = StartCoroutine(AsyncLoadRoutine(sceneName, scene));
      return scene;
    }

    private IEnumerator AsyncLoadRoutine(SceneId sceneName, PreloadedScene preloadedScene) {
      yield return _currentLoadRoutine;

      var asyncOperation = SceneManager.LoadSceneAsync(sceneName.SceneName());
      asyncOperation.allowSceneActivation = false;
      while (!asyncOperation.isDone) {
        if (preloadedScene.IsActivated()) {
          yield return FadeOut();
          asyncOperation.allowSceneActivation = true;
        }
        yield return null;
      }
      yield return FadeIn();
    }

    private IEnumerator SynchronousLoadRoutine(SceneId scene) {
      // for now, fully fade out before starting scene load. This is due to the tendency
      // for the code to block on scene load (e.g. generating an entire encounter), which
      // interrupts the fade. This should be fixed in the future.
      // TODO(P2): Fix async loading to not block this way, and add more loading indicators.
      yield return FadeOut();
      var asyncOperation = SceneManager.LoadSceneAsync(scene.SceneName());
      yield return new WaitUntil(() => asyncOperation.isDone);
      yield return FadeIn();
    }
    
    private IEnumerator FadeOut() {
      return Transition(Color.clear, Color.black, fadeOutDuration);
    }
    
    private IEnumerator FadeIn() {
      return Transition(Color.black, Color.clear, fadeInDuration);
    }

    private IEnumerator Transition(Color startColor, Color endColor, float duration) {
      var elapsedTime = 0f;
      while (elapsedTime < duration) {
        _fade.style.backgroundColor = new StyleColor(Color.Lerp(startColor, endColor, elapsedTime / duration));
        yield return null;
        elapsedTime += Time.deltaTime;
      }
      _fade.style.backgroundColor = new StyleColor(endColor);
    }
  }
}