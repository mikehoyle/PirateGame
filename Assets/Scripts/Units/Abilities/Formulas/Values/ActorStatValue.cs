using System;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class ActorStatValue : IDerivedValue {
    [SerializeField] private Stat stat;
    
    public float GetValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      return context.Actor.EncounterState.metadata.GetStat(stat);
    }
    public string DisplayString() => stat.displayName;
  }
}