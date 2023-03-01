using System;

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
  }
}