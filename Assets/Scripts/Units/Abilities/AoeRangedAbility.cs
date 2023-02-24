﻿using Encounters;
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
        Vector3Int source,
        GameObject hoveredObject,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      if (range.IsInRange(actor, source, hoveredTile)) {
        indicators.TargetingIndicator.TargetAoe(_areaOfEffect.WithTarget(hoveredTile));
        return;
      }
      indicators.TargetingIndicator.Clear();
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      return range.IsInRange(context.Actor, context.Source, context.TargetedTile);
    }
    
    protected override void Execute(AbilityExecutionContext context) {
      encounterEvents.applyAoeEffect.Raise(_areaOfEffect.WithTarget(context.TargetedTile), incurredEffect);
      // TODO(P1): Account for animation time
      encounterEvents.abilityExecutionEnd.Raise();
    }
  }
}