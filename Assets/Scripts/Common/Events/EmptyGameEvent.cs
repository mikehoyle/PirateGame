using UnityEngine;

namespace Common.Events {
  [CreateAssetMenu(menuName = "Events/EmptyGameEvent")]
  public class EmptyGameEvent : GameEvent<EmptyGameEvent.OnEventRaised> {
    public delegate void OnEventRaised();

    public void Raise() {
      foreach (var listener in Listeners) {
        listener.Invoke();
      }
    }
  }
}