using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Encounters.Enemies;
using Encounters.Grid;
using Optional;
using RuntimeVars;
using RuntimeVars.Encounters;
using StaticConfig.Units;
using Terrain;
using Units.Abilities;
using UnityEngine;

namespace Encounters.AI {
  // TODO(P1): This is far too rudimentary, but it's what we've got for now.
  public class AiActionEvaluator : MonoBehaviour {
    [SerializeField] private UnitCollection playerUnits;
    [SerializeField] private EnemyUnitCollection enemyUnits;
    [SerializeField] private ExhaustibleResource movementResource;
    [SerializeField] private MoveAbility moveAbility;

    private SceneTerrain _terrain;
    private GridIndicators _indicators;

    private static class ActionPreferences {
      public const float PlayerUnitAdjacency = 5f;
      public const float PlayerUnitDistanceFalloff = 0.1f;
      public const float CanPerformAbility = 10f;
    }

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _indicators = GridIndicators.Get();
    }

    public AiActionPlan GetActionPlan(EnemyUnitController enemy, List<Vector3Int> claimedTiles) {
      var possibleDestinations = _terrain.GetAllViableDestinations(
          enemy.Position, enemy.EncounterState.GetResourceAmount(movementResource));
      var abilities = enemy.GetAllCapableAbilities();

      var bestActionPlan = new AiActionPlan(enemy);
      var bestScore = 0f;
      
      foreach (var destination in possibleDestinations.Append(enemy.Position)) {
        if (claimedTiles.Contains(destination)) {
          // Never choose to walk where someone already decided to walk. As with everything else here,
          // this is janky and not ideal, but it'll do for now.
          continue;
        }
        foreach (var playerUnit in playerUnits) {
          foreach (var ability in abilities) {
            // TODO(P1): this ignores any obstacles, and could result in dumb AI that never moves around
            //     obstacles. Use the pathfinder for actual proximity.
            var distanceFromPlayer = GridUtils.DistanceBetween(destination, playerUnit.Position) - 1;
            var score = ActionPreferences.PlayerUnitAdjacency
                - (distanceFromPlayer * ActionPreferences.PlayerUnitDistanceFalloff);
            score = Math.Max(score, 0);
            var currentActionPlan = new AiActionPlan(enemy) {
                MoveDestination = destination,
            };
            var originalPosition = enemy.Position;
            enemy.Position = destination; 
            var executionContext = new UnitAbility.AbilityExecutionContext {
                Actor = enemy,
                Indicators = _indicators,
                TargetedObject = playerUnit.gameObject,
                TargetedTile = playerUnit.Position,
                Terrain = _terrain,
            };
            if (ability.CouldExecute(executionContext)) {
              score += ActionPreferences.CanPerformAbility;
              currentActionPlan.Action = Option.Some(new AiActionPlan.AiAction() {
                  Ability = ability,
                  Context = executionContext,
              });
            }
            enemy.Position = originalPosition;
            if (score > bestScore) {
              bestScore = score;
              bestActionPlan = currentActionPlan;
            }
          }
        }
      }

      return bestActionPlan;
    }
  }
}