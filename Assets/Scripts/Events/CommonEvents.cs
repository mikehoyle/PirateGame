using Common.Events;
using UnityEngine;

namespace Events {
  public class CommonEvents {
    public readonly GameEvent DialogueStart = new();
    public readonly GameEvent DialogueEnd = new();
    public readonly GameEvent<GameObject> ObjectClicked = new();
  }
}