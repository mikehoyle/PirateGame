using System;
using StaticConfig.Units;
using UnityEngine;

namespace Encounters.Effects {
  [CreateAssetMenu(menuName = "Encounters/Effects/StatusEffect")]
  public class StatusEffect : ScriptableObject {
    [Serializable]
    public class ExhaustibleResourceEffect {
      public ExhaustibleResource resource;
      public int diff;
    }

    public ExhaustibleResourceEffect[] exhaustibleResourceEffects;

    /// <summary>
    /// Because status effects can have individual tracking mechanisms, to apply them,
    /// we duplicate and initialize each instance.
    /// </summary>
    public StatusEffect Apply() {
      var result = Instantiate(this);
      result.OnApply();
      return result;
    }

    /// <returns>If the effect was destroyed</returns>
    public virtual bool UpdateAndMaybeDestroy(EncounterActor victim) {
      return false;
    }

    public virtual void OnApply() { }

    protected void EnactEffect(EncounterActor victim) {
      Debug.Log($"Applying effect to to victim {victim.name}");
      foreach (var exhaustibleResourceEffect in exhaustibleResourceEffects) {
        victim.EncounterState.ExpendResource(
            exhaustibleResourceEffect.resource, -exhaustibleResourceEffect.diff);
      }
    }
  }
}