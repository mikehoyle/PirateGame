using Common.Events;
using Encounters.Effects;
using Units.Abilities.AOE;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/AoeEffectEvent")]
  public class AoeEffectEvent : ParameterizedGameEvent<AreaOfEffect, StatusEffectInstanceFactory> {}
}