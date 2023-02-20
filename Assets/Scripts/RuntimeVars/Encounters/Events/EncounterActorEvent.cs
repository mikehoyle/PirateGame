using Common.Events;
using Encounters;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/EncounterActorEvent")]
  public class EncounterActorEvent : ParameterizedGameEvent<EncounterActor> {}
}