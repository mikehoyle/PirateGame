﻿using System.Collections;
using Encounters;
using Encounters.Grid;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/Abilities/MoveAbility")]
  public class MoveAbility : UnitAbility {
    [SerializeField] private ExhaustibleResource movementResource;

    public override void ShowIndicator(
        EncounterActor actor,
        Vector3Int source,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      indicators.RangeIndicator.DisplayMovementRange(
          source, actor.EncounterState.GetResourceAmount(movementResource), actor.EncounterState.faction);
      indicators.PathIndicator.DisplayMovementPath(
          actor.Position,
          actor.EncounterState.GetResourceAmount(movementResource),
          hoveredTile,
          actor.EncounterState.faction);
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      var path = context.Terrain.GetPath(
          context.Actor.Position, context.TargetedTile, context.Actor.EncounterState.faction);
      return path.IsViableAndWithinRange(context.Actor.EncounterState.GetResourceAmount(movementResource));
    }

    protected override IEnumerator Execute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      var path = context.Terrain.GetPath(
          context.Actor.Position, context.TargetedTile, context.Actor.EncounterState.faction);
      if (!path.IsViableAndWithinRange(context.Actor.EncounterState.GetResourceAmount(movementResource))) {
        Debug.LogWarning("Path became non-viable during movement execution. This should not happen");
        callback();
        yield break;
      }

      yield return context.Actor.MoveAlongPath(path);
      callback();
    }
  }
}