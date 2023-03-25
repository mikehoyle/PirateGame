using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Common.Events {
  public abstract class BaseGameEvent<T> where T : Delegate {
    protected readonly List<T> Listeners;
    
    protected BaseGameEvent() {
      Listeners = new();
      SceneManager.sceneUnloaded -= OnSceneUnloaded;
      SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene _) {
      // Whenever the scene changes, clear any straggling scene-dependent listeners
      // that may have forgotten to remove themselves. Currently just remove everything,
      // it's the easiest way and there aren't any cross-scene listeners (yet).
      Listeners.Clear();
    }

    public void RegisterListener(T listener) {
      Listeners.Add(listener);
    }
    
    public void UnregisterListener(T listener) {
      Listeners.Remove(listener);
    }
  }
}