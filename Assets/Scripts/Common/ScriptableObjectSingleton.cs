using UnityEngine;

namespace Common {
  public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T> {
    public static T Instance { get; private set; }
    
    protected virtual void Awake() {
      Instance ??= Self();
    }

    protected virtual void OnEnable() {
      Instance ??= Self();
      // Enable this if needed, but it may cause memory leaks according to docs, so don't use now.
      // hideFlags |= HideFlags.DontUnloadUnusedAsset;
    }

    protected virtual void OnDisable() {
      Instance = null;
    }

    protected abstract T Self();
  }
}