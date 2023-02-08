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

    public override bool TryExecute(UnitController actor, GameObject clickedObject, Vector3Int targetTile) {
      // TODO(P0): big fat TODO
      Debug.Log($"Attempting to execute ability: {id}");
      return false;
    }
  }
}