using System.Collections;
using System.Collections.Generic;
using Common;
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
      if (enemiesInEncounter.Count == 0) {
        encounterEvents.enemyTurnPreEnd.Raise();
        return;
      }

      Coroutine previousUnitAction = null;
      foreach (var enemy in enemiesInEncounter) {
        var actionPlan = _evaluator.GetActionPlan(enemy);
        previousUnitAction = StartCoroutine(ExecuteAction(enemy, actionPlan, previousUnitAction));
      }

      StartCoroutine(EndAiTurn(previousUnitAction));
    }

    private IEnumerator ExecuteAction(EncounterActor enemy, AiActionPlan actionPlan, Coroutine previousUnitAction) {
      yield return previousUnitAction;
      var path = _terrain.GetPath(actionPlan.Actor.Position, actionPlan.MoveDestination);
      yield return enemy.MoveAlongPath(path);
      if (actionPlan.Action.TryGet(out var action)) {
        if (action.Ability.TryExecute(action.Context, () => { }).TryGet(out var abilityExecution)) {
          yield return StartCoroutine(abilityExecution);
        }
      }
    }

    private IEnumerator EndAiTurn(Coroutine lastUnitAction) {
      yield return lastUnitAction;
      encounterEvents.enemyTurnPreEnd.Raise();
    }
  }
}