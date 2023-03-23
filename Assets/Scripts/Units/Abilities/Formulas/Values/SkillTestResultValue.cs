using System;
using Encounters;

namespace Units.Abilities.Formulas.Values {
  [Serializable]
  public class SkillTestResultValue : IDerivedValue {
    public float GetValue(EncounterActor _, float skillTestResult) {
      return skillTestResult;
    }
    
    public string DisplayString() => "[skill test]";
  }
}