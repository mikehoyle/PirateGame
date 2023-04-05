using System;
using System.Linq;
using System.Text;
using StaticConfig.Units;

namespace Encounters.Effects {
  [Serializable]
  public class ModifyStatsStatusEffect : StatusEffect {
    public SerializableDictionary<Stat, int> statModifiers;

    public override AppliedStatusEffect ApplyTo(EncounterActor victim) {
      var component = victim.StatusEffects.AddComponent<AppliedModifyStatsStatusEffect>();
      component.Initialize(this);
      return component;
    }

    public override string DisplayString() {
      var result = new StringBuilder();
      result.AppendJoin(
          ", ",
          statModifiers.Select(
              modifier => result.Append(
                  $"{(modifier.Value > 0 ? "+" : "")}{modifier.Value} {modifier.Key.displayName}")));
      return result.ToString();
    }
  }
}