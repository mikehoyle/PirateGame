using System;
using Encounters;
using UnityEngine;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class FloatValue : IDerivedValue {
    [SerializeField] private float value;
    
    public float GetValue(EncounterActor _, float skillTestResult) {
      return GetValueNoContext();
    }

    public float GetValueNoContext() {
      return value;
    }
    
    public string DisplayString() => value.ToString("F1");
  }
}