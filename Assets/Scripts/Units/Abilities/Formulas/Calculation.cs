using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Abilities.Formulas {
  [Serializable]
  public class Calculation {
    [Serializable]
    public enum Operation {
      Add = 0,
      Subtract = 1,
      Multiply = 2,
      Divide = 3,
    }

    public Operation operation;
    public List<DerivedValue> operands;

    public float CalculateValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      return operation switch {
          Operation.Subtract => ExecuteOperation(context, skillTestResult, (a, b) => a - b),
          // If I divide by zero using this, it's my own damn fault.
          Operation.Divide => ExecuteOperation(context, skillTestResult, (a, b) => a / b),
          Operation.Multiply => ExecuteOperation(context, skillTestResult, (a, b) => a * b),
          // Default to Add
          _ => ExecuteOperation(context, skillTestResult, (a, b) => a + b),
      };
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
  }
}