using UnityEngine;

namespace Common {
  public class GlobalSingletons : MonoBehaviour {
    private static GlobalSingletons _instance;

    private void Awake() {
      if (!_instance) {
        _instance = this;
        DontDestroyOnLoad(gameObject);
      } if (_instance != this) {
        Destroy(gameObject);
      }
    }
  }
}