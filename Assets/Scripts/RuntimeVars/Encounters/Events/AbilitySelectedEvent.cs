using Common.Events;
using StaticConfig.Units;
using Units;
using Units.Abilities;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  /// <summary>
  /// Params: actor, ability, source
  /// </summary>
  [CreateAssetMenu(menuName = "Events/Encounters/AbilitySelected")]
  public class AbilitySelectedEvent : ParameterizedGameEvent<PlayerUnitController, UnitAbility, Vector3Int> { }
}