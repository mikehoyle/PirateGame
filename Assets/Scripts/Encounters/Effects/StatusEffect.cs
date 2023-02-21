using System;
using System.Text;
using StaticConfig.Units;
using Units.Abilities;
using Units.Abilities.Formulas;
using UnityEngine;

namespace Encounters.Effects {
  [CreateAssetMenu(menuName = "Encounters/Effects/StatusEffect")]
  public class StatusEffect : ScriptableObject {
    [Serializable]
    public class ExhaustibleResourceEffect {
      public ExhaustibleResource resource;
      public AggregateCalculation calculation;
      public float CalculatedValue { get; private set; }

      public void Calculate(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
        CalculatedValue = calculation.CalculateValue(context, skillTestResult);
      }
    }

    public ExhaustibleResourceEffect[] exhaustibleResourceEffects;

    public void CalculateEffects(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      foreach (var effect in exhaustibleResourceEffects) {
        effect.Calculate(context, skillTestResult);
      }
    }

    /// <summary>
    /// Because status effects can have individual tracking mechanisms, to apply them,
    /// we duplicate and initialize each instance.
    /// </summary>
    public StatusEffect Apply(EncounterActor victim) {
      var result = Instantiate(this);
      result.OnApply(victim);
      return result;
    }

    /// <returns>If the effect was destroyed</returns>
    public virtual bool UpdateAndMaybeDestroy(EncounterActor victim) {
      return false;
    }

    public string DisplayString() {
      var result = new StringBuilder();
      for (int i = 0; i < exhaustibleResourceEffects.Length; i++) {
        result.Append($"({exhaustibleResourceEffects[i].calculation.DisplayString()}) ");
        result.Append($"{exhaustibleResourceEffects[i].resource.displayName}");
        if (i != exhaustibleResourceEffects.Length - 1) {
          result.Append(", ");
        }
      }
      
      return result.ToString();
    }
    

    public virtual void OnApply(EncounterActor victim) { }

    protected virtual void EnactEffect(EncounterActor victim) {
      Debug.Log($"Applying effect {name} to to victim {victim.name}");
      foreach (var exhaustibleResourceEffect in exhaustibleResourceEffects) {
        victim.ExpendResource(
            exhaustibleResourceEffect.resource, (int)exhaustibleResourceEffect.CalculatedValue);
      }
    }
  }
}