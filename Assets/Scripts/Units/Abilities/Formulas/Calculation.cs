using System;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities.Formulas {
  /// <summary>
  /// The way this class is structured is because of the dumb way needed to make it editable in
  /// the inspector.
  /// </summary>
  [Serializable]
  public class Calculation {
    [Serializable]
    public enum Type {
      ConstInt = 0,
      ConstFloat = 1,
      CalculatedValue = 2,
      ActorExhaustibleResource = 3,
      ActorStat = 4,
      SkillTestResult = 5,
    }

    public Type type;
    
    // Only one of these should ever be filled
    public int constInt;
    public float constFloat;
    public ExhaustibleResource exhaustibleResource;
    public Stat stat;
    public CalculatedValue calculatedValue;

    public float GetValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      return type switch {
          Type.ConstInt => constInt,
          Type.ConstFloat => constFloat,
          Type.CalculatedValue => calculatedValue.CalculateValue(context, skillTestResult),
          Type.ActorExhaustibleResource => context.Actor.EncounterState.GetResourceAmount(exhaustibleResource),
          Type.ActorStat => context.Actor.EncounterState.GetStat(stat),
          Type.SkillTestResult => skillTestResult,
          // Should be unreachable
          _ => LogWarning(),
      };
    }

    private float LogWarning() {
      Debug.LogWarning("Unknown type used for Calculation");
      return 0f;
    }
  }
}