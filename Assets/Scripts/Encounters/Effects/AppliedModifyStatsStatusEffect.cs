using StaticConfig.Units;

namespace Encounters.Effects {
  public class AppliedModifyStatsStatusEffect : AppliedStatusEffect, IStatModifier {
    private ModifyStatsStatusEffect _statusEffect;

    public void Initialize(ModifyStatsStatusEffect statusEffect) {
      _statusEffect = statusEffect;
    }

    public int GetStatModifier(EncounterActor actor, Stat stat) {
      return _statusEffect.statModifiers.TryGetValue(stat, out var value) ? value : 0;
    }
  }
}