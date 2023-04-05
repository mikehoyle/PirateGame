using System;
using System.Collections.Generic;
using System.Text;
using Encounters;
using UnityEngine;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class CalculatedValue : IDerivedValue {
    [SerializeField] private Operation operation;
    [SerializeReference, SerializeReferenceButton] private List<IDerivedValue> operands;

    public float GetValue(EncounterActor actor, float skillTestResult) {
      if (operands.Count == 0) {
        Debug.LogWarning("Cannot perform operation with zero operands");
        return 0;
      }
      
      var result = operands[0].GetValue(actor, skillTestResult);
      for (int i = 1; i < operands.Count; i++) {
        result = operation.GetOperation()(result, operands[i].GetValue(actor, skillTestResult));
      }

      return result;
    }
    
    

    public float GetValueNoContext() {
      if (operands.Count == 0) {
        Debug.LogWarning("Cannot perform operation with zero operands");
        return 0;
      }
      
      var result = operands[0].GetValueNoContext();
      for (int i = 1; i < operands.Count; i++) {
        result = operation.GetOperation()(result, operands[i].GetValueNoContext());
      }

      return result;
    }
    
    public string DisplayString() {
      var result = new StringBuilder("(");

      for (int i = 0; i < operands.Count; i++) {
        result.Append(operands[i].DisplayString());
        if (i != operands.Count - 1) {
          result.Append(operation.DisplayString());
        }
      }
      
      result.Append(")");
      return result.ToString();
    }
  }
}