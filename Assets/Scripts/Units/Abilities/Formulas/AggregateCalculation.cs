using System;
using UnityEngine;

namespace Units.Abilities.Formulas {
  [Serializable]
  public class AggregateCalculation {
    public Calculation[] calculationsToSum;

    public float CalculateValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      if (calculationsToSum.Length == 0) {
        Debug.LogWarning("Cannot perform operation with zero calculations");
        return 0;
      }
      
      var result = calculationsToSum[0].CalculateValue(context, skillTestResult);
      for (int i = 1; i < calculationsToSum.Length; i++) {
        result += calculationsToSum[i].CalculateValue(context, skillTestResult);
      }

      return result;
    }
  }
}