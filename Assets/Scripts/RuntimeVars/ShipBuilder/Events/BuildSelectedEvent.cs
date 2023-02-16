using Common.Events;
using StaticConfig.Builds;
using UnityEngine;

namespace RuntimeVars.ShipBuilder.Events {
  [CreateAssetMenu(menuName = "Events/ShipBuilder/BuildSelectedEvent")]
  public class BuildSelectedEvent : ParameterizedGameEvent<ConstructableObject> { }
}