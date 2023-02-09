using Encounters.Enemies;
using Encounters.Grid;
using UnityEngine;
using static Common.GridUtils;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/Abilities/SingleTargetWithRangeAbility")]
  public class SingleTargetWithRangeAbility : UnitAbility {
    [SerializeField] private int rangeMin;
    [SerializeField] private int rangeMax;

    public override void OnSelected(UnitController actor, GridIndicators indicators) {
      indicators.RangeIndicator.DisplayTargetingRange(actor.Position, rangeMin, rangeMax);
    }

    public override void ShowIndicator(
        UnitController actor,
        GameObject hoveredObject,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      var target = GetTargetIfEligible(actor, hoveredObject);
      if (target != null) {
        indicators.TargetingIndicator.TargetTile(target.State.position);
        return;
      }
      indicators.TargetingIndicator.Clear();
    }

    public override bool TryExecute(AbilityExecutionContext context) {
      Debug.Log("Attempting to execute punch");
      var target = GetTargetIfEligible(context.Actor, context.TargetedObject);
      Debug.Log($"Target is null? {target == null}");
      if (target == null) {
        return false;
      }
      
      // TODO(P0) add effect system and apply it
      Debug.Log("Would be punchin that dude.");
      return true;
    }

    private EnemyUnitController GetTargetIfEligible(UnitController actor, GameObject target) {
      if (target == null) {
        return null;
      }
      Debug.Log($"Game component exists");
      if (target.TryGetComponent<EnemyUnitController>(out var enemyUnit)) {
        var distance = DistanceBetween(actor.Position, enemyUnit.State.position);
        Debug.Log($"Distance {distance}");
        if (distance <= rangeMax && distance >= rangeMin) {
          return enemyUnit;
        }
      }
      return null;
    }
  }
}