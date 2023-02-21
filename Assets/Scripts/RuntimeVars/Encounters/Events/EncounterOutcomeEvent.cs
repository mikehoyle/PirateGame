using Common.Events;
using Encounters;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/EncounterOutcome")]
  public class EncounterOutcomeEvent : ParameterizedGameEvent<EncounterOutcome> {}
}