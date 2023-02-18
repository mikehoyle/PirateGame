using System;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities.Formulas {
  /// <summary>
  /// The way this class is structured is because of the dumb way needed to make it editable in
  /// the inspector.
  /// </summary>
  [Serializable]
  public class DerivedValue {
    [Serializable]
    public enum Type {
      ConstInt,
      ConstFloat,
      ActorExhaustibleResource,
      ActorStat,
      SkillTestResult,
    }

    public Type type;
    
    // Only one of these should ever be filled
    public int constInt;
    public float constFloat;
    public ExhaustibleResource exhaustibleResource;
    public Stat stat;

    public float GetValue(UnitAbility.AbilityExecutionContext context, float skillTestResult) {
      return type switch {
          Type.ConstInt => constInt,
          Type.ConstFloat => constFloat,
          Type.ActorExhaustibleResource => context.Actor.EncounterState.GetResourceAmount(exhaustibleResource),
          Type.ActorStat => context.Actor.EncounterState.GetStat(stat),
          Type.SkillTestResult => skillTestResult,
          // Should be unreachable
          _ => LogWarning(),
      };
    }

    public string DisplayString() {
      return type switch {
          Type.ConstInt => constInt.ToString(),
          Type.ConstFloat => constFloat.ToString("F1"),
          Type.ActorExhaustibleResource => exhaustibleResource.displayName,
          Type.ActorStat => stat.displayName,
          Type.SkillTestResult => "[skill test]",
          // Should be unreachable
          _ => "?",
      };
    }

    private float LogWarning() {
      Debug.LogWarning("Unknown type used for Calculation");
      return 0f;
    }
  }
}