using Pathfinding;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using UnityEngine;

namespace Encounters.AI {
  public class EncounterAiManager : MonoBehaviour {
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private EnemyUnitCollection enemiesInEncounter;
    
    private AiActionEvaluator _evaluator;

    private void Awake() {
      _evaluator = GetComponent<AiActionEvaluator>();
    }

    private void OnEnable() {
      encounterEvents.enemyTurnStart.RegisterListener(OnEnemyTurnStart);
    }

    private void OnDisable() {
      encounterEvents.enemyTurnStart.UnregisterListener(OnEnemyTurnStart);
    }

    private void OnEnemyTurnStart() {
      foreach (var enemy in enemiesInEncounter) {
        var actionPlan = _evaluator.GetActionPlan(enemy);
        foreach (var actionPlanComponent in actionPlan) {
          Debug.Log($"Attempting to execute {actionPlanComponent.Ability.name} for AI");
          actionPlanComponent.Ability.TryExecute(actionPlanComponent.Context);
        }
      }
      
      // TODO(P0): Very big! wait for the action to be done first
      encounterEvents.enemyTurnEnd.Raise();
    }
  }
}