using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Events {
  public abstract class GameEvent<T> : ScriptableObject where T : Delegate {
    protected List<T> Listeners;
    
    private void Awake() {
      SceneManager.activeSceneChanged -= OnSceneChanged;
      SceneManager.activeSceneChanged += OnSceneChanged;
#if UNITY_EDITOR
      UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
      UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
#endif
    }

    private void OnDestroy() {
      SceneManager.activeSceneChanged -= OnSceneChanged;
#if UNITY_EDITOR
      UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
#endif
    }

    private void OnValidate() {
      Awake();
    }

    private void OnSceneChanged(Scene a, Scene b) {
      // Whenever the scene changes, clear any straggling scene-dependent listeners
      // that may have forgotten to remove themselves. This shouldn't be relied upon,
      // but should clean up any missed corner cases.
      for (int i = Listeners.Count - 1; i >= 0; i--) {
        if (Listeners[i] == null) {
          Listeners.RemoveAt(i);
        }
      }
    }

    private void OnEnable() {
      Listeners = new();
    }

    private void OnDisable() {
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