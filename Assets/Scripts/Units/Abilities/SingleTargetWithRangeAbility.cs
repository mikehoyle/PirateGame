using Encounters;
using Encounters.Effects;
using Encounters.Grid;
using Encounters.SkillTest;
using RuntimeVars.Encounters.Events;
using UnityEngine;
using static Common.GridUtils;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/Abilities/SingleTargetWithRangeAbility")]
  public class SingleTargetWithRangeAbility : UnitAbility {
    [SerializeField] private int rangeMin;
    [SerializeField] private int rangeMax;
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private StatusEffect incurredEffect;

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
        var distance = DistanceBetween(actor.Position, targetUnit.EncounterState.position);
        if (distance <= rangeMax && distance >= rangeMin) {
          return targetUnit;
        }
      }
      return null;
    }

    private void OnDetermineAbilityEffectiveness(float result, EncounterActor target) {
      Debug.Log($"Skill test complete with result {result}");
      target.AddStatusEffect(Instantiate(incurredEffect));
      // TODO(P1): Account for animation time
      encounterEvents.abilityExecutionEnd.Raise();
    }
  }
}