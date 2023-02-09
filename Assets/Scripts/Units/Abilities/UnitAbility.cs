using System;
using Common;
using Common.Events;
using Encounters.Grid;
using Pathfinding;
using StaticConfig.Units;
using UnityEngine;

namespace Units.Abilities {
  /// <summary>
  /// Encapsulates an actionable capability of a unit.
  /// Expand on this greatly.
  /// </summary>
  public abstract class UnitAbility : EnumScriptableObject {
    [SerializeField] protected EmptyGameEvent beginAbilityExecutionEvent;
    [SerializeField] protected EmptyGameEvent endAbilityExecutionEvent;
    
    [Serializable]
    public struct UnitAbilityCost {
      public int amount;
      public ExhaustibleResource resource;
    }

    public class AbilityExecutionContext {
      public UnitController Actor { get; set; }
      public GameObject TargetedObject { get; set; }
      public Vector3Int TargetedTile { get; set; }
      public EncounterTerrain Terrain { get; set; }
      public GridIndicators Indicators { get; set; }
    }

    public string displayString;
    public UnitAbilityCost[] cost;

    public virtual void OnSelected(UnitController actor, GridIndicators indicators) { }
    
    public virtual void ShowIndicator(
        UnitController actor, GameObject hoveredObject, Vector3Int hoveredTile, GridIndicators indicators) { }

    /// <returns>Whether the ability is successfully executing</returns>
    public abstract bool TryExecute(AbilityExecutionContext context);
  }
}