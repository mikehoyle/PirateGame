using Encounters;
using Encounters.Grid;
using UnityEngine;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/Abilities/SingleTargetRangedAbility")]
  public class SingleTargetRangedAbility : RangedAbility {

    public override void ShowIndicator(
        EncounterActor actor,
        GameObject hoveredObject,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      var target = GetTargetIfEligible(actor, hoveredObject);
      if (target != null) {
        indicators.TargetingIndicator.TargetTile(target.EncounterState.position);
        return;
      }
      indicators.TargetingIndicator.Clear();
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      if (!CanAfford(context.Actor)) {
        return false;
      }
      var target = GetTargetIfEligible(context.Actor, context.TargetedObject);
      if (target == null) {
        return false;
      }
      
      return true;
    }

    public override bool TryExecute(AbilityExecutionContext context) {
      if (!CanAfford(context.Actor)) {
        return false;
      }
      var target = GetTargetIfEligible(context.Actor, context.TargetedObject);
      if (target == null) {
        return false;
      }
      
      encounterEvents.abilityExecutionStart.Raise();
      SpendCost(context.Actor);
      DetermineAbilityEffectiveness(context.Actor, result => OnDetermineAbilityEffectiveness(result, target));
      return true;
    }

    private EncounterActor GetTargetIfEligible(EncounterActor actor, GameObject target) {
      if (target == null) {
        return null;
      }
      if (target.TryGetComponent<EncounterActor>(out var targetUnit)) {
        if (targetUnit.EncounterState.faction == actor.EncounterState.faction) {
          return null;
        }
        if (IsInRange(actor.Position, targetUnit.EncounterState.position)) {
          return targetUnit;
        }
      }
      return null;
    }

    private void OnDetermineAbilityEffectiveness(float result, EncounterActor target) {
      target.AddStatusEffect(incurredEffect);
      // TODO(P1): Account for animation time
      encounterEvents.abilityExecutionEnd.Raise();
    }
  }
}