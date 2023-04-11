using System;
using StaticConfig.Units;
using Units.Abilities.Formulas.Values;

namespace Encounters.Effects {
  /// <summary>
  /// Applies an effect a single time, after an optional animation delay.
  /// </summary>
  [Serializable]
  public class OneShotStatusEffect : ExhaustibleResourceStatusEffect {
    public float delayInSeconds;
    
    public override AppliedStatusEffect ApplyTo(EncounterActor victim) {
      var component = victim.StatusEffects.AddComponent<AppliedOneShotStatusEffect>();
      component.Initialize(this, victim);
      return component;
    }

    public static OneShotStatusEffect SimpleDamageEffect(int damage) {
      return new OneShotStatusEffect {
          delayInSeconds = 0,
          exhaustibleResourceEffects = new[] {
              new ExhaustibleResourceEffect {
                  resource = ExhaustibleResources.Instance.hp,
                  value = new IntValue(damage),
              },
          },
      };
    }
  }
}