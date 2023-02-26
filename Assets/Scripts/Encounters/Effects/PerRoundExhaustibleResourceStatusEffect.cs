using System;
using System.Collections.Generic;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Effects {
  [Serializable]
  public class PerRoundExhaustibleResourceStatusEffect : PerRoundStatusEffect {
    public ExhaustibleResourceEffect[] exhaustibleResourceEffects;
    
    public override IStatusEffectInstance NewInstance(EncounterActor victim) {
      return new PerRoundExhaustibleResourceStatusEffectInstance(this, victim);
    }
    
    public class PerRoundExhaustibleResourceStatusEffectInstance : PerRoundStatusEffectInstance {
      private Dictionary<ExhaustibleResource, float> _calculatedEffects;

      public PerRoundExhaustibleResourceStatusEffectInstance(
          PerRoundExhaustibleResourceStatusEffect sourceEffect, EncounterActor victim)
          : base(sourceEffect, victim) {}

      public void PreCalculateEffect(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
        _calculatedEffects.Clear();
        var sourceEffect = (PerRoundExhaustibleResourceStatusEffect)_sourceEffect;
        foreach (var effect in sourceEffect.exhaustibleResourceEffects) {
          _calculatedEffects.Add(effect.resource, effect.value.GetValue(context, skillTestResult));
        }
      }

      public override void EnactEffect(EncounterActor victim) {
        if (_calculatedEffects.Count == 0) {
          Debug.LogWarning("No effects to enact, did you forget to call PreCalculateEffect?");
          return;
        }
        Debug.Log($"Applying round-start effect to to victim {victim.name}");
        foreach (var exhaustibleResourceEffect in _calculatedEffects) {
          victim.ExpendResource(exhaustibleResourceEffect.Key, (int)exhaustibleResourceEffect.Value);
        }
      }
    }
  }
}