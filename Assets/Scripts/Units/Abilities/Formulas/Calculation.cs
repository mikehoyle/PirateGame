using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Abilities.Formulas {
  [Serializable]
  public class Calculation {
    // Aggregation operation is how the end result of this Calculation is applied to other aggregated values
    public Operation aggregationOperation;
    // Internal operation is the operation applied between the members within this Calculation.
    [FormerlySerializedAs("operation")] public Operation internalOperation;
    public List<DerivedValue> operands;

    public float CalculateValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      return ExecuteOperation(context, skillTestResult, internalOperation.GetOperation());
    }

    private float ExecuteOperation(
        UnitAbility.AbilityExecutionContext context, float skillTestResult, Func<float, float, float> op) {
      if (operands.Count == 0) {
        Debug.LogWarning("Cannot perform operation with zero operands");
        return 0;
      }
      
      var result = operands[0].GetValue(context, skillTestResult);
      for (int i = 1; i < operands.Count; i++) {
        result = op(result, operands[i].GetValue(context, skillTestResult));
      }

      return result;
    }

    public string DisplayString() {
      var result = new StringBuilder("(");

      for (int i = 0; i < operands.Count; i++) {
        result.Append(operands[i].DisplayString());
        if (i != operands.Count - 1) {
          result.Append(internalOperation.DisplayString());
        }
      }
      
      result.Append(")");
      return result.ToString();
    }
  }
}