using System.Collections;
using Common.Animation;
using Encounters;
using Encounters.Enemies;
using Encounters.Grid;
using Optional;
using Optional.Unsafe;
using State.Unit;
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
      if (!range.IsInRange(actor, source, hoveredTile)) {
        return;
      }
      var target = MaybeGetTarget(actor, hoveredObject);
      var spiritTarget = MaybeGetSpiritTarget(actor, hoveredObject);
      if (target != null) {
        indicators.TargetingIndicator.TargetTiles(hoveredTile);
        return;
      }
      if (spiritTarget != null) {
        indicators.TargetingIndicator.TargetTiles(hoveredTile, spiritTarget.GetPushTarget(source));
        return;
      }
      
      indicators.TargetingIndicator.Clear();
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      if (!CanAfford(context.Actor) || !range.IsInRange(context.Actor, context.Source, context.TargetedTile)) {
        return false;
      }
      var target = MaybeGetTarget(context.Actor, context.TargetedObject);
      var spiritTarget = MaybeGetSpiritTarget(context.Actor, context.TargetedObject);

      return target != null || spiritTarget != null;
    }

    protected override IEnumerator Execute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      var target = MaybeGetTarget(context.Actor, context.TargetedObject);
      var spiritTarget = MaybeGetSpiritTarget(context.Actor, context.TargetedObject);
      if (target == null && spiritTarget == null) {
        Debug.LogWarning("Could not find target when executing");
        callback();
        yield break;
      }

      if (target != null) {
        yield return ExecuteOnTarget(target, context);
      }
      
      if (spiritTarget != null) {
        yield return ExecuteOnSpirit(spiritTarget, context);
      }
      
      callback();
    }

    private IEnumerator ExecuteOnTarget(EncounterActor target, AbilityExecutionContext context) {
      var skillTestResult = Option.None<float>();
      DetermineAbilityEffectiveness(context.Actor, result => skillTestResult = Option.Some(result));
      yield return new WaitUntil(() => skillTestResult.HasValue);
      
      var effect = incurredEffect.ApplyTo(target);
      effect.PreCalculateEffect(context.Actor, skillTestResult.ValueOrFailure());
      // Animation options should definitely not be here... a future problem.
      context.Actor.FaceTowards(target.Position);
      context.Actor.PlayOneOffAnimation(AnimationNames.Attack);
      PlaySound();
      yield return new WaitForSeconds(impactAnimationDelaySec);
      yield return CreateImpactAnimation(target.Position);
    }

    private IEnumerator ExecuteOnSpirit(SpiritUnitController spirit, AbilityExecutionContext context) {
      yield return spirit.Push(FacingUtilities.DirectionBetween(context.Source, spirit.Position));
    }

    private EncounterActor MaybeGetTarget(
        EncounterActor actor, GameObject target) {
      if (target == null) {
        return null;
      }
      if (target.TryGetComponent<EncounterActor>(out var targetUnit)) {
        if (targetUnit.EncounterState.faction == actor.EncounterState.faction && !canTargetAllies) {
          return null;
        }
        if (targetUnit.EncounterState.faction != actor.EncounterState.faction && !canTargetOpponents) {
          return null;
        }
        return targetUnit;
      }
      return null;
    }

    private SpiritUnitController MaybeGetSpiritTarget(EncounterActor actor, GameObject target) {
      // For now, only players can directly target spirits, in the future this may become a parameter.
      if (target == null || actor.EncounterState.faction != UnitFaction.PlayerParty) {
        return null;
      }
      return target.GetComponent<SpiritUnitController>();
    }
  }
}