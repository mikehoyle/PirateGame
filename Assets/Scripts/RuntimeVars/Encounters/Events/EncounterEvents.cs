using Common.Events;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  [CreateAssetMenu(menuName = "Events/Encounters/EncounterEvents")]
  public class EncounterEvents : ScriptableObject {
    public EmptyGameEvent abilityExecutionStart;
    public EmptyGameEvent abilityExecutionEnd;
    public AbilitySelectedEvent abilitySelected;
    public EmptyGameEvent encounterStart;
    public EmptyGameEvent enemyTurnStart;
    public EmptyGameEvent enemyTurnEnd;
    public EmptyGameEvent playerTurnStart;
    public Vector3Event mouseHover;
    public EmptyGameEvent newRound;
    public ObjectClickedEvent objectClicked;
    public EmptyGameEvent playerTurnEnd;
    public UnitSelectedEvent unitSelected;
    public AoeEffectEvent applyAoeEffect;
  }
}