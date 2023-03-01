using System;

namespace Encounters.Effects {
  [Serializable]
  public class PerRoundExhaustibleResourceStatusEffect : PerRoundStatusEffect {
    public ExhaustibleResourceEffect[] exhaustibleResourceEffects;

    public override AppliedStatusEffect ApplyTo(EncounterActor victim) {
      var component = victim.StatusEffects.AddComponent<AppliedPerRoundExhaustibleResourceStatusEffect>();
      component.Initialize(this, victim);
      return component;
    }
  }
}