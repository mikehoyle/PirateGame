using UnityEngine;

namespace Common {
  public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T> {
    public static T Instance { get; private set; }

    private void OnEnable() {
      Instance = Self();
    }

    private void OnDisable() {
      Instance = null;
    }

    protected abstract T Self();
  }
}