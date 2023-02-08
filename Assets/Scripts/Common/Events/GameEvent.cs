using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Events {
  public abstract class GameEvent<T> : ScriptableObject where T : Delegate {
    protected List<T> Listeners;

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