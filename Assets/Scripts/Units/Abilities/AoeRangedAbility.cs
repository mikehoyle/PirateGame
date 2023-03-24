using System.Collections;
using System.Collections.Generic;
using Common.Animation;
using Encounters;
using Encounters.Effects;
using Encounters.Grid;
using Events;
using Optional;
using Optional.Unsafe;
using State.Unit;
using Units.Abilities.AOE;
using UnityEngine;

namespace Units.Abilities {
  /// <summary>
  /// Defines a ranged ability that can target an area.
  /// </summary>
  [CreateAssetMenu(menuName = "Units/Abilities/AoeRangedAbility")]
  public class AoeRangedAbility : RangedAbility {
    [SerializeField] [Multiline] private string aoeDefinition;
    private AreaOfEffect _areaOfEffect;

    private void Awake() {
      UpdateAoeDefinition();
    }

    private void OnValidate() {
      UpdateAoeDefinition();
    }

    private void UpdateAoeDefinition() {
      if (aoeDefinition == null) {
        return;
      }
      _areaOfEffect = AoeParser.ParseAreaOfEffect(aoeDefinition);
    }
    
    public override void ShowIndicator(
        EncounterActor actor,
        Vector3Int source,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      base.ShowIndicator(actor, source, hoveredTile, indicators);
      indicators.TargetingIndicator.Clear();
      if (range.IsInRange(actor, source, hoveredTile)) {
        indicators.TargetingIndicator.TargetAoe(_areaOfEffect.WithTargetAndRotation(source, hoveredTile));
      }
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      return range.IsInRange(context.Actor, context.Source, context.TargetedTile);
    }
    
    protected override IEnumerator Execute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      var aoe = _areaOfEffect.WithTargetAndRotation(context.Source, context.TargetedTile);
      
      var skillTestResult = Option.None<float>();
      DetermineAbilityEffectiveness(context.Actor, result => skillTestResult = Option.Some(result));
      yield return new WaitUntil(() => skillTestResult.HasValue);

      var affectedFactions = new List<UnitFaction>();
      if (canTargetAllies) {
        affectedFactions.Add(context.Actor.EncounterState.faction);
      }
      if (canTargetOpponents) {
        affectedFactions.Add(context.Actor.EncounterState.OpposingFaction());
      }
      
      var instanceFactory = new StatusEffectApplier(
          incurredEffect, context, affectedFactions, skillTestResult.ValueOrFailure());
      Dispatch.Encounters.ApplyAoeEffect.Raise(aoe, instanceFactory);
      // Animation options should definitely not be here... a future problem.
      context.Actor.FaceTowards(aoe.GetTarget());
      context.Actor.PlayOneOffAnimation(AnimationNames.Attack);
      yield return new WaitForSeconds(impactAnimationDelaySec);
      yield return CreateImpactAnimation(context.TargetedTile);
      PlaySound();
      // TODO(P1): Account for animation time
      callback();
    }
  }
}