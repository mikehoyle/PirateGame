using System;
using System.Collections.Generic;
using System.Linq;
using Common.Animation;
using Common.Events;
using Common.Grid;
using Encounters.AI;
using Encounters.Grid;
using Events;
using HUD.Encounter.HoverDetails;
using Optional;
using RuntimeVars.Encounters;
using State.Unit;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Enemies {
  public class EnemyUnitController : EncounterActor {
    [SerializeField] protected EnemyUnitCollection enemiesInEncounter;
    
    protected DirectionalAnimator _animator;
    protected List<UnitAbility> _abilities;
    protected GridIndicators _gridIndicators;

    public override UnitEncounterState EncounterState { get; protected set; }
    public EnemyUnitMetadata Metadata => (EnemyUnitMetadata)EncounterState.metadata;
    protected override GameEvent TurnPreStartEvent => Dispatch.Encounters.EnemyTurnPreStart;

    protected override void Awake() {
      base.Awake();
      _animator = GetComponent<DirectionalAnimator>();
      _gridIndicators = GridIndicators.Get();
    }

    protected override void OnEnable() {
      base.OnEnable();
      enemiesInEncounter.Add(this);
      Dispatch.Encounters.UnitSelected.RegisterListener(OnUnitSelected);
    }

    protected override void OnDisable() {
      base.OnDisable();
      enemiesInEncounter.Remove(this);
      Dispatch.Encounters.UnitSelected.UnregisterListener(OnUnitSelected);
    }

    public AiActionPlan GetActionPlan(ActionEvaluationContext context) {
      var possibleDestinations = context.Terrain.GetAllViableDestinations(
          Position, EncounterState.GetResourceAmount(exhaustibleResources.mp), context.ClaimedTileOverrides)
          .Append(Position);
      var abilities = GetAllCapableAbilities();

      var bestActionPlan = new AiActionPlan(this);
      var bestScore = float.MinValue;
      
      foreach (var destination in possibleDestinations) {
        foreach (var playerUnit in context.PlayerUnits.Concat<EncounterActor>(context.EnemyUnits)) {
          foreach (var ability in abilities) {
            // TODO(P1): this ignores any obstacles, and could result in dumb AI that never moves around
            //     obstacles. Use the pathfinder for actual proximity.
            var distanceFromPlayer = GridUtils.DistanceBetween(destination, playerUnit.Position) - 1;
            var score = 0f;
            if (distanceFromPlayer == 1) {
              score += Metadata.actionPreferences.playerUnitAdjacency;
            }
            score += (distanceFromPlayer * Metadata.actionPreferences.distanceFromPlayerByTile);
            score = Math.Max(score, 0);
            if (destination == Position) {
              score += Metadata.actionPreferences.stayStationary;
            }
            if (Metadata.actionPreferences.allyRadius > 0) {
              foreach (var enemyUnit in context.EnemyUnits) {
                if (enemyUnit == this) {
                  continue;
                }
                if (GridUtils.DistanceBetween(destination, enemyUnit.Position)
                    <= Metadata.actionPreferences.allyRadius) {
                  score += Metadata.actionPreferences.inRadiusOfAlly;
                }
              }
            }

            var currentActionPlan = new AiActionPlan(this) {
                MoveDestination = destination,
            }; 
            var executionContext = new UnitAbility.AbilityExecutionContext {
                Actor = this,
                Source = destination,
                Indicators = context.Indicators,
                TargetedObject = playerUnit.gameObject,
                TargetedTile = playerUnit.Position,
                Terrain = context.Terrain,
            };
            if (ability.CouldExecute(executionContext)) {
              score += Metadata.actionPreferences.canPerformAbility;
              currentActionPlan.Action = Option.Some(new AiActionPlan.AiAction {
                  Ability = ability,
                  Context = executionContext,
              });
            }
            if (score > bestScore) {
              bestScore = score;
              bestActionPlan = currentActionPlan;
            }
          }
        }
      }

      return bestActionPlan;
    }
    
    protected override void InitInternal(UnitEncounterState encounterState) {
      if (encounterState.metadata is not EnemyUnitMetadata enemyUnitMetadata) {
        Debug.LogWarning("Enemy units need to be initialized with enemy unit state");
        return;
      }
      _animator.SetSprite(enemyUnitMetadata.sprite);
    }

    private void OnUnitSelected(EncounterActor selectedUnit) {
      if (selectedUnit != null
          && this == selectedUnit
          && EncounterState.TryGetResourceTracker(exhaustibleResources.mp, out var movementRange)) {
        _gridIndicators.RangeIndicator.DisplayMovementRange(Position, movementRange.max);
      }
    }

    protected override void OnDeath() {
      enemiesInEncounter.Remove(this);
      PlayOneOffAnimation("death");
      // TODO(P1): Account for animation time
      Destroy(gameObject);
    }

    public override DisplayDetails GetDisplayDetails() {
      var result = base.GetDisplayDetails();
      result.AdditionalDetails.Add(Metadata.shortDescription);
      return result;
    }
  }
}