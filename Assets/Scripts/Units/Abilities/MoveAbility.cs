﻿using Encounters;
using Encounters.Grid;
using RuntimeVars.Encounters.Events;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/Abilities/MoveAbility")]
  public class MoveAbility : UnitAbility {
    [SerializeField] private ExhaustibleResource movementResource;
    
    public override void OnSelected(EncounterActor actor, GridIndicators indicators, Vector3Int source) {
      indicators.RangeIndicator.DisplayMovementRange(source, actor.EncounterState.GetResourceAmount(movementResource));
    }

    public override void ShowIndicator(
        EncounterActor actor,
        Vector3Int source,
        GameObject hoveredObject,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      indicators.PathIndicator.DisplayMovementPath(
          actor.Position,
          actor.EncounterState.GetResourceAmount(movementResource),
          hoveredTile);
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      var path = context.Terrain.GetPath(context.Actor.Position, context.TargetedTile);
      return path.IsViableAndWithinRange(context.Actor.EncounterState.GetResourceAmount(movementResource));
    }

    protected override void Execute(AbilityExecutionContext context) {
      var path = context.Terrain.GetPath(context.Actor.Position, context.TargetedTile);
      if (!path.IsViableAndWithinRange(context.Actor.EncounterState.GetResourceAmount(movementResource))) {
        Debug.LogWarning("Path became non-viable during movement execution. This should not happene");
        return;
      }
      
      // Manually spend cost because it is dynamic.
      context.Actor.EncounterState.ExpendResource(movementResource, path.Length());
      context.Actor.MoveAlongPath(path, OnMoveComplete);
      encounterEvents.abilityExecutionStart.Raise();
    }

    private void OnMoveComplete() {
      encounterEvents.abilityExecutionEnd.Raise();
    }
  }
}