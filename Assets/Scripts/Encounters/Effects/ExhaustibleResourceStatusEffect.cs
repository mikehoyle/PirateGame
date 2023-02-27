﻿using System;
using System.Text;
using StaticConfig.Units;
using Units.Abilities.Formulas.Values;
using UnityEngine;

namespace Encounters.Effects {
  /// <summary>
  /// Super class for any effect affecting exhaustible resources, such as dealing damage.
  /// </summary>
  [Serializable]
  public abstract class ExhaustibleResourceStatusEffect : StatusEffect {
    [Serializable]
    public class ExhaustibleResourceEffect {
      public ExhaustibleResource resource;
      [SerializeReference, SerializeReferenceButton]
      public IDerivedValue value;
    }
    
    public ExhaustibleResourceEffect[] exhaustibleResourceEffects;

    public override string DisplayString() {
      var result = new StringBuilder();
      for (int i = 0; i < exhaustibleResourceEffects.Length; i++) {
        result.Append($"({exhaustibleResourceEffects[i].value.DisplayString()}) ");
        result.Append($"{exhaustibleResourceEffects[i].resource.displayName}");
        if (i != exhaustibleResourceEffects.Length - 1) {
          result.Append(", ");
        }
      }
      
      return result.ToString();
    }
  }
}