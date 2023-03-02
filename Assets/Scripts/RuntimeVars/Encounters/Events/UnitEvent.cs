using Common.Events;
using Encounters;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/UnitEvent")]
  public class UnitEvent : ParameterizedGameEvent<EncounterActor> {}
}