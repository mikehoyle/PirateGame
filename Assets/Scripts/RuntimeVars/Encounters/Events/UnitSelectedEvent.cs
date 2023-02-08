using Common;
using Common.Events;
using State;
using State.Unit;
using Units;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/UnitSelectedEvent")]
  public class UnitSelectedEvent : ParameterizedGameEvent<UnitController> {}
}