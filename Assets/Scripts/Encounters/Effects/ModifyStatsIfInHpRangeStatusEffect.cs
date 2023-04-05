using System;

namespace Encounters.Effects {
  [Serializable]
  public class ModifyStatsIfInHpRangeStatusEffect : ModifyStatsStatusEffect {
    public float minPercentHpForEffect;
    public float maxPercentHpForEffect;
    
    public override AppliedStatusEffect ApplyTo(EncounterActor victim) {
      var component = victim.StatusEffects.AddComponent<AppliedModifyStatsIfInHpRangeStatusEffect>();
      component.Initialize(this);
      return component;
    }
  }
}