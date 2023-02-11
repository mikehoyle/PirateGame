using Encounters;
using Encounters.Grid;
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
        GameObject hoveredObject,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      if (IsInRange(actor.Position, hoveredTile)) {
        indicators.TargetingIndicator.TargetAoe(_areaOfEffect.WithTarget(hoveredTile));
        return;
      }
      indicators.TargetingIndicator.Clear();
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      return IsInRange(context.Actor.Position, context.TargetedTile);
    }
    
    public override bool TryExecute(AbilityExecutionContext context) {
      if (!CouldExecute(context)) {
        return false;
      }
      // TODO Normalize all this stuff
      encounterEvents.abilityExecutionStart.Raise();
      SpendCost(context.Actor);
      encounterEvents.applyAoeEffect.Raise(_areaOfEffect.WithTarget(context.TargetedTile), incurredEffect);
      // TODO(P1): Account for animation time
      encounterEvents.abilityExecutionEnd.Raise();
      return true;
    }
  }
}