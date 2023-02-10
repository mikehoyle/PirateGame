using UnityEngine;

namespace Common.Events {
  [CreateAssetMenu(menuName = "Events/EmptyGameEvent")]
  public class EmptyGameEvent : GameEvent<EmptyGameEvent.OnEventRaised> {
    public delegate void OnEventRaised();

    public void Raise() {
      for (int i = Listeners.Count - 1; i >= 0; i--) {
        Listeners[i].Invoke();
      }
    }
  }
}