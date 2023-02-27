using System;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class ExhaustibleResourceValue : IDerivedValue {
    [SerializeField] private ExhaustibleResource resource;
    
    public float GetValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      return context.Actor.EncounterState.GetResourceAmount(resource);
    }
    
    public string DisplayString() => resource.displayName;
  }
}