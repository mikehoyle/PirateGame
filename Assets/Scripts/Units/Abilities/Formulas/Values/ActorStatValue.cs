using System;
using Encounters;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class ActorStatValue : IDerivedValue {
    [SerializeField] private Stat stat;
    
    public float GetValue(EncounterActor actor, float skillTestResult) {
      return actor.GetStat(stat);
    }

    public float GetValueNoContext() {
      return 0;
    }
    
    public string DisplayString() => stat.displayName;
  }
}