using System;
using UnityEngine;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class FloatValue : IDerivedValue {
    [SerializeField] private float value;
    
    public float GetValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      return value;
    }
    
    public string DisplayString() => value.ToString("F1");
  }
}