using Encounters;
using Encounters.Grid;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/Abilities/MoveAbility")]
  public class MoveAbility : UnitAbility {
    public override void OnSelected(UnitController actor, GridIndicators indicators) {
      indicators.RangeIndicator.DisplayMovementRange(actor);
    }

    public override void ShowIndicator(
        UnitController actor, GameObject hoveredObject, Vector3Int hoveredTile, GridIndicators indicators) {
      indicators.PathIndicator.DisplayMovementPath(actor, hoveredTile);
    }

    public override bool TryExecute(AbilityExecutionContext context) {
      var path = context.Terrain.GetPath(context.Actor.Position, context.TargetedTile);

      if (!path.IsViableAndWithinRange(context.Actor.State.encounterState.remainingMovement)) {
        return false;
      }
      
      if (context.Actor.MoveAlongPath(path, OnMoveComplete)) {
        beginAbilityExecutionEvent.Raise();
        context.Indicators.Clear();
        
        // TODO(P1): Convert to using MP resource
        context.Actor.State.encounterState.remainingMovement -= path.Length();
        return true;
      }
      return false;
    }

    private void OnMoveComplete() {
      endAbilityExecutionEvent.Raise();
    }
  }
}