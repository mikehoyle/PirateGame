using System;
using Common;
using Encounters;
using Encounters.Grid;
using Units;
using UnityEngine;

namespace StaticConfig.Units {
  /// <summary>
  /// Encapsulates an actionable capability of a unit.
  /// Expand on this greatly.
  /// </summary>
  public abstract class UnitAbility : EnumScriptableObject {
    [Serializable]
    public struct UnitAbilityCost {
      public int amount;
      public ExhaustibleResource resource;
    }

    public string displayString;
    public UnitAbilityCost[] cost;

    /// <returns>Whether the ability is successfully executing</returns>
    public abstract bool TryExecute(UnitController actor, GameObject clickedObject, Vector3Int targetTile);

    public virtual void OnSelected(UnitController actor, GridIndicators indicators) { }
    
    public virtual void ShowIndicator(
        UnitController actor, GameObject hoveredObject, Vector3Int hoveredTile, GridIndicators indicators) { }
  }
}