using StaticConfig.Units;

namespace Encounters.Effects {
  public class AppliedModifyStatsIfInHpRangeStatusEffect : AppliedStatusEffect, IStatModifier {
    private ModifyStatsIfInHpRangeStatusEffect _statusEffect;

    public void Initialize(ModifyStatsIfInHpRangeStatusEffect statusEffect) {
      _statusEffect = statusEffect;
    }

    public int GetStatModifier(EncounterActor actor, Stat stat) {
      var actorHp = actor.EncounterState.GetResourceAmount(ExhaustibleResources.Instance.hp);
      if (actorHp >= _statusEffect.minPercentHpForEffect && actorHp <= _statusEffect.maxPercentHpForEffect) {
        return _statusEffect.statModifiers.TryGetValue(stat, out var value) ? value : 0;
      }

      return 0;
    }
  }
}