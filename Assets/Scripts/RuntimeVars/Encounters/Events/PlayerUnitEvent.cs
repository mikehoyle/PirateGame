using Common.Events;
using Units;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/PlayerUnitEvent")]
  public class PlayerUnitEvent : ParameterizedGameEvent<UnitController> {}
}