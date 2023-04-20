using Common.Events;
using UnityEngine;

namespace Events {
  public class CommonEvents {
    public readonly GameEvent<GameObject> ObjectClicked = new();
  }
}