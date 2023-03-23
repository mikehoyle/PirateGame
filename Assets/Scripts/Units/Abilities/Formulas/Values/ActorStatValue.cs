using System;
using Encounters;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class ActorStatValue : IDerivedValue {
    [SerializeField] private Stat stat;
    
    public float GetValue(EncounterActor actor, float skillTestResult) {
      return actor.EncounterState.metadata.GetStat(stat);
    }
    public string DisplayString() => stat.displayName;
  }
}