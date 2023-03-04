using System.Collections;
using Common.Animation;
using Encounters;
using Encounters.Effects;
using Encounters.Grid;
using Optional;
using Optional.Unsafe;
using Units.Abilities.AOE;
using UnityEngine;

namespace Units.Abilities {
  /// <summary>
  /// Defines a ranged ability that can target an area.
  /// </summary>
  [CreateAssetMenu(menuName = "Units/Abilities/AoeRangedAbility")]
  public class AoeRangedAbility : RangedAbility {
    [SerializeField] private TextAsset aoeDefinition;
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
        GameObject hoveredObject,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      if (range.IsInRange(actor, source, hoveredTile)) {
        indicators.TargetingIndicator.TargetAoe(_areaOfEffect.WithTargetAndRotation(source, hoveredTile));
        return;
      }
      indicators.TargetingIndicator.Clear();
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      return range.IsInRange(context.Actor, context.Source, context.TargetedTile);
    }
    
    protected override IEnumerator Execute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      var aoe = _areaOfEffect.WithTargetAndRotation(context.Source, context.TargetedTile);
      
      var skillTestResult = Option.None<float>();
      DetermineAbilityEffectiveness(context.Actor, result => skillTestResult = Option.Some(result));
      yield return new WaitUntil(() => skillTestResult.HasValue);
      
      var instanceFactory = new StatusEffectApplier(incurredEffect, context, skillTestResult.ValueOrFailure());
      encounterEvents.applyAoeEffect.Raise(aoe, instanceFactory);
      // Animation options should definitely not be here... a future problem.
      context.Actor.FaceTowards(aoe.GetTarget());
      context.Actor.PlayOneOffAnimation(AnimationNames.Attack);
      PlaySound();
      // TODO(P1): Account for animation time
      callback();
    }
  }
}