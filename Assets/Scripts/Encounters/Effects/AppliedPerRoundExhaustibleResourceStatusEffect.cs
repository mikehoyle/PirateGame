using System.Collections;
using System.Collections.Generic;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Effects {
  public class AppliedPerRoundExhaustibleResourceStatusEffect : AppliedPerRoundStatusEffect {
    private readonly Dictionary<ExhaustibleResource, float> _calculatedImmediateEffects = new();
    private readonly Dictionary<ExhaustibleResource, float> _calculatedPerRoundEffects = new();
    
    public override void PreCalculateEffect(EncounterActor actor, float skillTestResult) {
      _calculatedImmediateEffects.Clear();
      _calculatedPerRoundEffects.Clear();
      var sourceEffect = (PerRoundExhaustibleResourceStatusEffect)_sourceEffect;
      foreach (var effect in sourceEffect.immediateEffects) {
        _calculatedImmediateEffects.Add(effect.resource, effect.value.GetValue(actor, skillTestResult));
      }
      foreach (var effect in sourceEffect.exhaustibleResourceEffects) {
        _calculatedPerRoundEffects.Add(effect.resource, effect.value.GetValue(actor, skillTestResult));
      }
    }

    protected override void EnactEffect() {
      EnactResourceEffects(_calculatedPerRoundEffects);
    }

    protected override void OnInitialize() {
      StartCoroutine(ApplyImmediateAfterDelay());
    }

    private IEnumerator ApplyImmediateAfterDelay() {
      yield return new WaitForSeconds(
          ((PerRoundExhaustibleResourceStatusEffect)_sourceEffect).immediateEffectDelaySecs);
      EnactResourceEffects(_calculatedImmediateEffects);
    }

    private void EnactResourceEffects(Dictionary<ExhaustibleResource, float> calculatedEffects) {
      if (calculatedEffects.Count == 0) {
        Debug.LogWarning("No effects to enact, did you forget to call PreCalculateEffect?");
        return;
      }
      Debug.Log($"Applying round-start effect to to victim {_victim.name}");
      foreach (var exhaustibleResourceEffect in calculatedEffects) {
        _victim.ExpendResource(exhaustibleResourceEffect.Key, (int)exhaustibleResourceEffect.Value);
      }
    }
  }
}