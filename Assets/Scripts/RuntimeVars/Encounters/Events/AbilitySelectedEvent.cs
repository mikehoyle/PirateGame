using Common.Events;
using StaticConfig.Units;
using Units;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/AbilitySelected")]
  public class AbilitySelectedEvent : ParameterizedGameEvent<UnitController, UnitAbility> { }
}