using UnityEngine;

namespace Common {
  public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T> {
    public static T Instance { get; private set; }
    
    private void Awake() {
      Instance ??= Self();
    }

    private void OnEnable() {
      Instance ??= Self();
      // Enable this if needed, but it may cause memory leaks according to docs, so don't use now.
      // hideFlags |= HideFlags.DontUnloadUnusedAsset;
    }

    private void OnDisable() {
      Instance = null;
    }

    protected abstract T Self();
  }
}