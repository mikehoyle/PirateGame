using Common.Animation;
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
      Debug.Log($"Was target determined eligible? {target}");
      if (target == null) {
        return false;
      }
      
      return true;
    }

    protected override void Execute(AbilityExecutionContext context) {
      var target = GetTargetIfEligible(context.Actor, context.TargetedObject);
      if (target == null) {
        Debug.LogWarning("Could not find target when executing");
        return;
      }
      
      DetermineAbilityEffectiveness(
          context.Actor, result => OnDetermineAbilityEffectiveness(context, result, target));
    }

    private EncounterActor GetTargetIfEligible(EncounterActor actor, GameObject target) {
      Debug.Log($"Is target eligible? {target?.name}");
      if (target == null) {
        return null;
      }
      if (target.TryGetComponent<EncounterActor>(out var targetUnit)) {
        if (targetUnit.EncounterState.faction == actor.EncounterState.faction) {
          Debug.Log($"a");
          return null;
        }
        if (IsInRange(actor.Position, targetUnit.EncounterState.position)) {
          Debug.Log($"b");
          return targetUnit;
        }
      }
      Debug.Log($"c");
      return null;
    }

    private void OnDetermineAbilityEffectiveness(
        AbilityExecutionContext context, float result, EncounterActor target) {
      var effect = incurredEffect.Apply(target);
      effect.CalculateEffects(context, result);
      target.AddStatusEffect(effect);
      // Animation options should definitely not be here... a future problem.
      context.Actor.FaceTowards(target.Position);
      context.Actor.PlayOneOffAnimation(AnimationNames.Attack);
      // TODO(P1): Account for animation time
      encounterEvents.abilityExecutionEnd.Raise();
    }
  }
}