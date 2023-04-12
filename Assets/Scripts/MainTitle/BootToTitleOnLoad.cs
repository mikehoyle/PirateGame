using Common.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainTitle {
  /// <summary>
  /// Quick and dirty class to boot out to the title scene on load.
  /// </summary>
  public class BootToTitleOnLoad : MonoBehaviour {
    private static bool _alreadyBooted = false;

    private void Awake() {
      if (!_alreadyBooted) {
        _alreadyBooted = true;
        SceneManager.LoadScene(Scenes.Name.Title.SceneName());
      }
    }
  }
}