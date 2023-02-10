using System;
using System.Collections.Generic;
using Common;
using Encounters.Enemies;
using Encounters.Grid;
using Pathfinding;
using RuntimeVars;
using RuntimeVars.Encounters;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace Encounters.AI {
  // TODO(P1): This is far too rudimentary, but it's what we've got for now.
  public class AiActionEvaluator : MonoBehaviour {
    [SerializeField] private UnitCollection playerUnits;
    [SerializeField] private EnemyUnitCollection enemyUnits;
    [SerializeField] private ExhaustibleResource movementResource;
    [SerializeField] private MoveAbility moveAbility;

    private EncounterTerrain _terrain;
    private GridIndicators _indicators;

    private static class ActionPreferences {
      public const float PlayerUnitProximity = 1f;
      public const float CanPerformAbility = 10f;
    }

    public class ActionPlanComponent {
      public UnitAbility Ability;
      public UnitAbility.AbilityExecutionContext Context;
    } 

    private void Awake() {
      _terrain = EncounterTerrain.Get();
      _indicators = GridIndicators.Get();
    }

    public List<ActionPlanComponent> GetActionPlan(EnemyUnitController enemy) {
      var possibleDestinations = _terrain.GetAllViableDestinations(
          enemy.Position, enemy.EncounterState.GetResourceAmount(movementResource));
      var abilities = enemy.GetAllCapableAbilities();

      var bestActionPlan = new List<ActionPlanComponent>();
      var bestScore = 0f;
      
      foreach (var destination in possibleDestinations) {
        foreach (var playerUnit in playerUnits) {
          foreach (var ability in abilities) {
            // TODO(P1): this ignores any obstacles, and could result in dumb AI that never moves around
            //     obstacles. Use the pathfinder for actual proximity.
            var score =
                GridUtils.DistanceBetween(enemy.Position, playerUnit.Position) * ActionPreferences.PlayerUnitProximity;
            var currentActionPlan = new List<ActionPlanComponent>() {
                new() {
                    Ability = moveAbility,
                    Context = new UnitAbility.AbilityExecutionContext {
                        Actor = enemy,
                        Indicators = _indicators,
                        TargetedObject = null,
                        TargetedTile = destination,
                        Terrain = _terrain,
                    }
                },
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
              currentActionPlan.Add(new() {
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