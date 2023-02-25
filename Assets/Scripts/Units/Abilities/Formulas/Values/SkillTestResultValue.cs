using System;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class SkillTestResultValue : IDerivedValue {
    public float GetValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      return skillTestResult;
    }
    
    public string DisplayString() => "[skill test]";
  }
}