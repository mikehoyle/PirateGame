using System.Collections.Generic;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Effects {
  public class AppliedOneShotStatusEffect : AppliedStatusEffect {
    private OneShotStatusEffect _sourceEffect;
    private float _startTime;
    private Dictionary<ExhaustibleResource, float> _calculatedEffects;
    private EncounterActor _victim;

    public void Initialize(OneShotStatusEffect sourceEffect, EncounterActor victim) {
      _startTime = 0;
      _sourceEffect = sourceEffect;
      _victim = victim;
      _calculatedEffects = new();
    }

    public override void PreCalculateEffect(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      _calculatedEffects.Clear();
      foreach (var effect in _sourceEffect.exhaustibleResourceEffects) {
        _calculatedEffects.Add(effect.resource, effect.value.GetValue(context, skillTestResult));
      }
    }
    
    private void Update() {
      _startTime += Time.deltaTime;
      if (_startTime >= _sourceEffect.delayInSeconds) {
        EnactEffect();
        Destroy(this);
      }
    }
      
    private void EnactEffect() {
      if (_calculatedEffects.Count == 0) {
        Debug.LogWarning("No effects to enact, did you forget to call PreCalculateEffect?");
        return;
      }
      Debug.Log($"Applying one-shot effect to to victim {_victim.name}");
      foreach (var exhaustibleResourceEffect in _calculatedEffects) {
        _victim.ExpendResource(exhaustibleResourceEffect.Key, (int)exhaustibleResourceEffect.Value);
      }
    }
  }
}