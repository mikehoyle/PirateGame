using Common.Events;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/EncounterEvents")]
  public class EncounterEvents : ScriptableObject {
    public EmptyGameEvent abilityExecutionStart;
    public EmptyGameEvent abilityExecutionEnd;
    public IntegerGameEvent trySelectAbilityByIndex;
    public AbilitySelectedEvent abilitySelected;
    public EmptyGameEvent encounterReadyToStart;
    public EmptyGameEvent encounterStart;
    public EncounterOutcomeEvent encounterEnd;
    public EmptyGameEvent enemyTurnPreStart;
    public EmptyGameEvent enemyTurnStart;
    public EmptyGameEvent enemyTurnEnd;
    public EmptyGameEvent playerTurnPreStart;
    public EmptyGameEvent playerTurnStart;
    public Vector3Event mouseHover;
    public EmptyGameEvent newRound;
    public ObjectClickedEvent objectClicked;
    public EmptyGameEvent playerTurnEnd;
    public PlayerUnitEvent unitSelected;
    public AoeEffectEvent applyAoeEffect;
    public EncounterActorEvent unitAddedMidEncounter;
    public EmptyGameEvent unitDeath;
  }
}