using Common.Events;
using Optional;
using Units;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/UnitDeathEvent")]
  public class UnitDeathEvent : ParameterizedGameEvent<Option<Bones>> {}
}