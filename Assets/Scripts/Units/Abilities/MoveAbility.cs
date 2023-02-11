using Encounters;
using Encounters.Grid;
using RuntimeVars.Encounters.Events;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/Abilities/MoveAbility")]
  public class MoveAbility : UnitAbility {
    [SerializeField] private ExhaustibleResource movementResource;
    
    public override void OnSelected(EncounterActor actor, GridIndicators indicators) {
      indicators.RangeIndicator.DisplayMovementRange(
          actor.Position, actor.EncounterState.GetResourceAmount(movementResource));
    }

    public override void ShowIndicator(
        EncounterActor actor, GameObject hoveredObject, Vector3Int hoveredTile, GridIndicators indicators) {
      indicators.PathIndicator.DisplayMovementPath(
          actor.Position,
          actor.EncounterState.GetResourceAmount(movementResource),
          hoveredTile);
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      var path = context.Terrain.GetPath(context.Actor.Position, context.TargetedTile);
      return path.IsViableAndWithinRange(context.Actor.EncounterState.GetResourceAmount(movementResource));
    }

    public override bool TryExecute(AbilityExecutionContext context) {
      var path = context.Terrain.GetPath(context.Actor.Position, context.TargetedTile);
      if (!path.IsViableAndWithinRange(context.Actor.EncounterState.GetResourceAmount(movementResource))) {
        return false;
      }

      context.Actor.MoveAlongPath(path, OnMoveComplete);
      encounterEvents.abilityExecutionStart.Raise();
      context.Actor.EncounterState.ExpendResource(movementResource, path.Length());
      return true;
    }

    private void OnMoveComplete() {
      encounterEvents.abilityExecutionEnd.Raise();
    }
  }
}