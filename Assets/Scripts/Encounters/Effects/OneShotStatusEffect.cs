using System;
using System.Collections.Generic;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Effects {
  /// <summary>
  /// Applies an effect a single time, after an optional animation delay.
  /// </summary>
  [Serializable]
  public class OneShotStatusEffect : ExhaustibleResourceStatusEffect {
    public float delayInSeconds;
    
    public override IStatusEffectInstance NewInstance(EncounterActor victim) {
      return new OneShotStatusEffectInstance(this);
    }
    
    public class OneShotStatusEffectInstance : IStatusEffectInstance {
      private readonly OneShotStatusEffect _sourceEffect;
      private float _startTime;
      private Dictionary<ExhaustibleResource, float> _calculatedEffects;

      public OneShotStatusEffectInstance(OneShotStatusEffect sourceEffect) {
        _startTime = 0;
        _sourceEffect = sourceEffect;
        _calculatedEffects = new();
      }

      public void PreCalculateEffect(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
        _calculatedEffects.Clear();
        foreach (var effect in _sourceEffect.exhaustibleResourceEffects) {
          _calculatedEffects.Add(effect.resource, effect.value.GetValue(context, skillTestResult));
        }
      }
      
      public bool UpdateAndMaybeDestroy(EncounterActor victim) {
        _startTime += Time.deltaTime;
        if (_startTime >= _sourceEffect.delayInSeconds) {
          EnactEffect(victim);
          return true;
        }
        return false;
      }
      
      public void EnactEffect(EncounterActor victim) {
        if (_calculatedEffects.Count == 0) {
          Debug.LogWarning("No effects to enact, did you forget to call PreCalculateEffect?");
          return;
        }
        Debug.Log($"Applying one-shot effect to to victim {victim.name}");
        foreach (var exhaustibleResourceEffect in _calculatedEffects) {
          victim.ExpendResource(exhaustibleResourceEffect.Key, (int)exhaustibleResourceEffect.Value);
        }
      }
    }
  }
}