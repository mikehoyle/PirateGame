using Common.Events;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/ObjectClickedEvent")]
  public class ObjectClickedEvent : ParameterizedGameEvent<GameObject> { }
}