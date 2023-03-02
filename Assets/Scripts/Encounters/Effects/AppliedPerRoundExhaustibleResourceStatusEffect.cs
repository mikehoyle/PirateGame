using System.Collections.Generic;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Effects {
  public class AppliedPerRoundExhaustibleResourceStatusEffect : AppliedPerRoundStatusEffect {
    private readonly Dictionary<ExhaustibleResource, float> _calculatedEffects = new();
    
    public override void PreCalculateEffect(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      _calculatedEffects.Clear();
      var sourceEffect = (PerRoundExhaustibleResourceStatusEffect)_sourceEffect;
      foreach (var effect in sourceEffect.exhaustibleResourceEffects) {
        _calculatedEffects.Add(effect.resource, effect.value.GetValue(context, skillTestResult));
      }
    }

    protected override void EnactEffect() {
      if (_calculatedEffects.Count == 0) {
        Debug.LogWarning("No effects to enact, did you forget to call PreCalculateEffect?");
        return;
      }
      Debug.Log($"Applying round-start effect to to victim {_victim.name}");
      foreach (var exhaustibleResourceEffect in _calculatedEffects) {
        _victim.ExpendResource(exhaustibleResourceEffect.Key, (int)exhaustibleResourceEffect.Value);
      }
    }
  }
}