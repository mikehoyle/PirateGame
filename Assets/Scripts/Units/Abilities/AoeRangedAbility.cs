using System.Collections;
using Encounters;
using Encounters.Effects;
using Encounters.Grid;
using Events;
using Optional;
using Optional.Unsafe;
using Units.Abilities.AOE;
using UnityEngine;
using Common;

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
      if (GetRange(actor).IsInRange(actor, source, hoveredTile)) {
        var aoe = GetAreaOfEffect(actor);
        indicators.TargetingIndicator.TargetAoe(aoe.WithTargetAndRotation(source, hoveredTile));
      }
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      return GetRange(context.Actor).IsInRange(context.Actor, context.Source, context.TargetedTile);
    }
    
    protected override IEnumerator Execute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      var aoe = GetAreaOfEffect(context.Actor).WithTargetAndRotation(context.Source, context.TargetedTile);
      
      var skillTestResult = Option.None<float>();
      DetermineAbilityEffectiveness(context.Actor, result => skillTestResult = Option.Some(result));
      yield return new WaitUntil(() => skillTestResult.HasValue);

      yield return fx.Execute(
          context,
          GetAffectedFactions(context.Actor),
          Option.Some(aoe),
          () => {
            var instanceFactory = new StatusEffectApplier(
                incurredEffect, Option.Some(context.Actor), GetAffectedFactions(context.Actor), skillTestResult.ValueOrFailure());
            Dispatch.Encounters.ApplyAoeEffect.Raise(aoe, instanceFactory);
          },
          () => callback());
    }

    private AreaOfEffect GetAreaOfEffect(EncounterActor actor) {
      if (actor is not PlayerUnitController playerUnit) {
        return _areaOfEffect;
      }

      var currentTier = 0;
      var result = _areaOfEffect;
      foreach (var upgrade in playerUnit.Metadata.GetAllUpgrades()) {
        // Sloppy logic that enforces at least higher-tier upgrades always override lower-tier upgrades.
        if (upgrade.GetUpgradeTier() > currentTier && upgrade.GetAoeOverride(playerUnit.Metadata, this).TryGet(out var aoeOverride)) {
          currentTier = upgrade.GetUpgradeTier();
          result = aoeOverride;
        }
      }
      return result;
    }
  }
}