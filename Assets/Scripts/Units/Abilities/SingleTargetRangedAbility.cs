using System.Collections;
using Common.Animation;
using Encounters;
using Encounters.Enemies;
using Encounters.Enemies.Spirits;
using Encounters.Grid;
using Optional;
using Optional.Unsafe;
using State.Unit;
using Terrain;
using Units.Abilities.AOE;
using UnityEngine;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/Abilities/SingleTargetRangedAbility")]
  public class SingleTargetRangedAbility : RangedAbility {
    public override void ShowIndicator(
        EncounterActor actor,
        Vector3Int source,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      base.ShowIndicator(actor, source, hoveredTile, indicators);

      indicators.TargetingIndicator.Clear();
      if (!GetRange(actor).IsInRange(actor, source, hoveredTile)) {
        return;
      }
      var target = MaybeGetTarget(actor, hoveredTile);
      var spiritTarget = MaybeGetSpiritTarget(actor, hoveredTile);
      if (target != null) {
        indicators.TargetingIndicator.TargetTiles(hoveredTile);
        return;
      }
      if (spiritTarget != null) {
        indicators.TargetingIndicator.TargetTiles(hoveredTile, spiritTarget.GetPushTarget(source));
        return;
      }
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      if (!CanAfford(context.Actor)
          || !GetRange(context.Actor).IsInRange(context.Actor, context.Source, context.TargetedTile)) {
        return false;
      }
      var target = MaybeGetTarget(context.Actor, context.TargetedTile);
      var spiritTarget = MaybeGetSpiritTarget(context.Actor, context.TargetedTile);

      return target != null || spiritTarget != null;
    }

    protected override IEnumerator Execute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      var target = MaybeGetTarget(context.Actor, context.TargetedTile);
      var spiritTarget = MaybeGetSpiritTarget(context.Actor, context.TargetedTile);
      if (target == null && spiritTarget == null) {
        Debug.LogWarning("Could not find target when executing");
        callback();
        yield break;
      }

      if (target != null) {
        yield return ExecuteOnTarget(target, context, callback);
        yield break;
      }
      
      if (spiritTarget != null) {
        yield return ExecuteOnSpirit(spiritTarget, context);
      }
      
      callback();
    }

    private IEnumerator ExecuteOnTarget(
        EncounterActor target, AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      var skillTestResult = Option.None<float>();
      DetermineAbilityEffectiveness(context.Actor, result => skillTestResult = Option.Some(result));
      yield return new WaitUntil(() => skillTestResult.HasValue);
      yield return fx.Execute(
          context,
          GetAffectedFactions(context.Actor),
          Option.None<AreaOfEffect>(),
          () => {
            var effect = incurredEffect.ApplyTo(target);
            effect.PreCalculateEffect(context.Actor, skillTestResult.ValueOrFailure());
          },
          () => {
            callback();
          });
    }

    private IEnumerator ExecuteOnSpirit(SpiritUnitController spirit, AbilityExecutionContext context) {
      yield return spirit.Push(FacingUtilities.DirectionBetween(context.Source, spirit.Position));
    }

    private EncounterActor MaybeGetTarget(EncounterActor actor, Vector3Int targetTile) {
      if (SceneTerrain.TryGetComponentAtTile<EncounterActor>(targetTile, out var targetUnit)) {
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

    private SpiritUnitController MaybeGetSpiritTarget(EncounterActor actor, Vector3Int targetTile) {
      // For now, only players can directly target spirits, in the future this may become a parameter.
      if (actor.EncounterState.faction != UnitFaction.PlayerParty) {
        return null;
      }
      SceneTerrain.TryGetComponentAtTile<SpiritUnitController>(targetTile, out var spirit);
      return spirit;
    }
  }
}