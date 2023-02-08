using System;
using Common;
using Units;
using UnityEngine;

namespace StaticConfig.Units {
  /// <summary>
  /// Encapsulates an actionable capability of a unit.
  /// Expand on this greatly.
  /// </summary>
  [CreateAssetMenu(menuName = "Units/Abilities/UnitAbility")]
  public class UnitAbility : EnumScriptableObject {
    [Serializable]
    public struct UnitAbilityCost {
      public int amount;
      public ExhaustibleResource resource;
    }

    public string displayString;
    public UnitAbilityCost[] cost;

    public void TryExecute(UnitController actor, Vector3Int targetTile) {
      // TODO(P0): big fat TODO
    }
    
    // Much more will go here.
  }
}