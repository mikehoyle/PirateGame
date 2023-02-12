using System.Collections.Generic;
using Optional.Unsafe;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using Terrain;
using UnityEngine;

namespace Encounters.AI {
  public class EncounterAiManager : MonoBehaviour {
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private EnemyUnitCollection enemiesInEncounter;
    
    private AiActionEvaluator _evaluator;
    private SceneTerrain _terrain;
    
    // Per-round state
    private int _enemyMovementsComplete;
    private List<AiActionPlan> _actionPlans;

    private void Awake() {
      _evaluator = GetComponent<AiActionEvaluator>();
      _terrain = SceneTerrain.Get();
    }

    private void OnEnable() {
      encounterEvents.enemyTurnStart.RegisterListener(OnEnemyTurnStart);
    }

    private void OnDisable() {
      encounterEvents.enemyTurnStart.UnregisterListener(OnEnemyTurnStart);
    }

    private void OnEnemyTurnStart() {
      _enemyMovementsComplete = 0;
      _actionPlans = new();
      var claimedTiles = new List<Vector3Int>();
      foreach (var enemy in enemiesInEncounter) {
        var actionPlan = _evaluator.GetActionPlan(enemy, claimedTiles);
        claimedTiles.Add(actionPlan.MoveDestination);
        _actionPlans.Add(actionPlan);
      }

      foreach (var actionPlan in _actionPlans) {
        var path = _terrain.GetPath(actionPlan.Actor.Position, actionPlan.MoveDestination);
        actionPlan.Actor.MoveAlongPath(path, OnCompleteMovement);
      }
    }

    private void OnCompleteMovement() {
      _enemyMovementsComplete += 1;
      if (_enemyMovementsComplete == enemiesInEncounter.Count) {
        PerformUnitActions();
      }
    }

    private void PerformUnitActions() {
      foreach (var actionPlan in _actionPlans) {
        if (actionPlan.Action.HasValue) {
          var ability = actionPlan.Action.ValueOrFailure();
          ability.Ability.TryExecute(ability.Context);
        }
      }
      // TODO(P0): Big! Actually wait for actions to complete
      encounterEvents.enemyTurnEnd.Raise();
    }
  }
}