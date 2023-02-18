using System;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Abilities.Formulas {
  [Serializable]
  public class AggregateCalculation {
    [FormerlySerializedAs("calculationsToSum")] public Calculation[] calculations;

    public float CalculateValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      if (calculations.Length == 0) {
        Debug.LogWarning("Cannot perform operation with zero calculations");
        return 0;
      }
      
      var result = calculations[0].CalculateValue(context, skillTestResult);
      for (int i = 1; i < calculations.Length; i++) {
        var operation = calculations[i].aggregationOperation.GetOperation();
        result = operation(result, calculations[i].CalculateValue(context, skillTestResult));
      }

      return result;
    }

    public string DisplayString() {
      var result = new StringBuilder();
      for (int i = 0; i < calculations.Length; i++) {
        if (i != 0) {
          result.Append(calculations[i].aggregationOperation.DisplayString());
        }
        result.Append(calculations[i].DisplayString());
      }
      return result.ToString();      
    }
  }
}