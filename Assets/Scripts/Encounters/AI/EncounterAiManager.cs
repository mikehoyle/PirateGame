﻿using System.Collections;
using System.Collections.Generic;
using Common;
using Encounters.Managers;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using Terrain;
using UnityEngine;

namespace Encounters.AI {
  public class EncounterAiManager : MonoBehaviour {
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private EnemyUnitCollection enemiesInEncounter;
    [SerializeField] private SpiritCollection spiritsInEncounter;
    
    private AiActionEvaluator _evaluator;
    private SceneTerrain _terrain;

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
      StartCoroutine(ExecuteEnemyAi());
    }
    private IEnumerator ExecuteEnemyAi() {
      if (enemiesInEncounter.Count == 0) {
        encounterEvents.enemyTurnPreEnd.Raise();
        yield break;
      }

      // Make all movements simultaneously
      var actionPlans = new List<AiActionPlan>();
      var enemyMovements = new List<Coroutine>();
      SparseMatrix3d<bool> claimedTileOverrides = new();
      foreach (var enemy in enemiesInEncounter.EnumerateByTurnPriority()) {
        var actionPlan = _evaluator.GetActionPlan(enemy, claimedTileOverrides);
        actionPlans.Add(actionPlan);
        var path = _terrain.GetPath(actionPlan.Actor.Position, actionPlan.MoveDestination);
        enemyMovements.Add(enemy.MoveAlongPath(path));
        claimedTileOverrides[actionPlan.Actor.Position] = true;
        claimedTileOverrides[actionPlan.MoveDestination] = false;
      }

      foreach (var enemyMovement in enemyMovements) {
        yield return enemyMovement;
      }
      
      // Then make actions sequentially
      foreach (var actionPlan in actionPlans) {
        yield return ExecuteAction(actionPlan.Actor, actionPlan);
      }
      
      
      // Let all spirits do their thing.
      for (int i = spiritsInEncounter.spirits.Count - 1; i >= 0; i--) {
        yield return spiritsInEncounter.spirits[i].ExecuteMovementPlan();
      }

      EndAiTurn();
    }

    private IEnumerator ExecuteAction(EncounterActor enemy, AiActionPlan actionPlan) {
      if (actionPlan.Action.TryGet(out var action)) {
        if (action.Ability.TryExecute(action.Context, () => { }).TryGet(out var abilityExecution)) {
          yield return StartCoroutine(abilityExecution);
        }
      }
    }

    private void EndAiTurn() {
      encounterEvents.enemyTurnPreEnd.Raise();
    }
  }
}