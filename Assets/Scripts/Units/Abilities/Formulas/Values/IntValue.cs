using System;
using Encounters;
using UnityEngine;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class IntValue : IDerivedValue {
    [SerializeField] private int value;

    public IntValue() { }
    public IntValue(int value) {
      this.value = value;
    }
    
    public float GetValue(EncounterActor _, float skillTestResult) {
      return GetValueNoContext();
    }

    public float GetValueNoContext() {
      return value;
    }
    public string DisplayString() {
      return value.ToString();
    }
  }
}