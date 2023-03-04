using Common.Animation;
using Encounters;
using Encounters.Grid;
using UnityEngine;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/Abilities/SingleTargetRangedAbility")]
  public class SingleTargetRangedAbility : RangedAbility {
    public override void ShowIndicator(
        EncounterActor actor,
        Vector3Int source,
        GameObject hoveredObject,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      var target = GetTargetIfEligible(actor, source, hoveredTile, hoveredObject);
      if (target != null) {
        indicators.TargetingIndicator.TargetTile(hoveredTile);
        return;
      }
      indicators.TargetingIndicator.Clear();
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      if (!CanAfford(context.Actor)) {
        return false;
      }
      var target = GetTargetIfEligible(
          context.Actor, context.Source, context.TargetedTile, context.TargetedObject);
      if (target == null) {
        return false;
      }
      
      return true;
    }

    protected override void Execute(AbilityExecutionContext context) {
      var target = GetTargetIfEligible(
          context.Actor, context.Source, context.TargetedTile, context.TargetedObject);
      if (target == null) {
        Debug.LogWarning("Could not find target when executing");
        return;
      }
      
      DetermineAbilityEffectiveness(
          context.Actor, result => OnDetermineAbilityEffectiveness(context, result, target));
    }

    private EncounterActor GetTargetIfEligible(
        EncounterActor actor, Vector3Int source, Vector3Int targetTile, GameObject target) {
      if (target == null) {
        return null;
      }
      if (target.TryGetComponent<EncounterActor>(out var targetUnit)) {
        if (targetUnit.EncounterState.faction == actor.EncounterState.faction) {
          return null;
        }
        if (range.IsInRange(actor, source, targetTile)) {
          return targetUnit;
        }
      }
      return null;
    }

    private void OnDetermineAbilityEffectiveness(
        AbilityExecutionContext context, float result, EncounterActor target) {
      var effect = incurredEffect.ApplyTo(target);
      effect.PreCalculateEffect(context, result);
      // Animation options should definitely not be here... a future problem.
      context.Actor.FaceTowards(target.Position);
      context.Actor.PlayOneOffAnimation(AnimationNames.Attack);
      PlaySound();
      // TODO(P1): Account for animation time
      encounterEvents.abilityExecutionEnd.Raise();
    }
  }
}