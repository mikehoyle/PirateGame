using System;
using UnityEngine;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class IntValue : IDerivedValue {
    [SerializeField] private int value;
    
    public float GetValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      return value;
    }
    public string DisplayString() {
      return value.ToString();
    }
  }
}