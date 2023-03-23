using System;
using Encounters;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class ExhaustibleResourceValue : IDerivedValue {
    [SerializeField] private ExhaustibleResource resource;
    
    public float GetValue(EncounterActor actor, float skillTestResult) {
      return actor.EncounterState.GetResourceAmount(resource);
    }
    
    public string DisplayString() => resource.displayName;
  }
}