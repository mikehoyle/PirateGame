using System.Collections;
using Common;
using Events;
using RuntimeVars.Encounters;
using Terrain;
using UnityEngine;

namespace Encounters.AI {
  public class EncounterAiManager : MonoBehaviour {
    [SerializeField] private EnemyUnitCollection enemiesInEncounter;
    [SerializeField] private SpiritCollection spiritsInEncounter;
    
    private AiActionEvaluator _evaluator;
    private SceneTerrain _terrain;

    private void Awake() {
      _evaluator = GetComponent<AiActionEvaluator>();
      _terrain = SceneTerrain.Get();
    }

    private void OnEnable() {
      Dispatch.Encounters.EnemyTurnStart.RegisterListener(OnEnemyTurnStart);
    }

    private void OnDisable() {
      Dispatch.Encounters.EnemyTurnStart.UnregisterListener(OnEnemyTurnStart);
    }

    private void OnEnemyTurnStart() {
      StartCoroutine(ExecuteEnemyAi());
    }
    private IEnumerator ExecuteEnemyAi() {
      // Let all spirits do their thing first.
      for (int i = spiritsInEncounter.Count - 1; i >= 0; i--) {
        yield return spiritsInEncounter[i].ExecuteMovementPlan();
      }

      if (enemiesInEncounter.Count == 0) {
        EndAiTurn();
        yield break;
      }

      SparseMatrix3d<bool> claimedTileOverrides = new();
      foreach (var enemy in enemiesInEncounter) {
        claimedTileOverrides.Add(enemy.Position, true);
      }

      foreach (var enemy in enemiesInEncounter.EnumerateByTurnPriority()) {
        var actionPlan = _evaluator.GetActionPlan(enemy, claimedTileOverrides);
        var path = _terrain.GetPath(
            actionPlan.Actor.Position, actionPlan.MoveDestination, actionPlan.Actor.EncounterState.faction);
        yield return enemy.MoveAlongPath(path);
        yield return ExecuteAction(actionPlan.Actor, actionPlan);
      }

      EndAiTurn();
    }

    private IEnumerator ExecuteAction(EncounterActor enemy, AiActionPlan actionPlan) {
      if (actionPlan.Action.TryGet(out var action)) {
        if (action.Ability.TryExecute(action.Context, () => { }).TryGet(out var abilityExecution)) {
          yield return abilityExecution;
        }
      }
    }

    private void EndAiTurn() {
      Dispatch.Encounters.EnemyTurnPreEnd.Raise();
    }
  }
}