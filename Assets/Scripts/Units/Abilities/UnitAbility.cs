using System;
using Common;
using Common.Events;
using Encounters;
using Encounters.Grid;
using Encounters.SkillTest;
using Pathfinding;
using State.Unit;
using StaticConfig.Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units.Abilities {
  /// <summary>
  /// Encapsulates an actionable capability of a unit.
  /// Expand on this greatly.
  /// </summary>
  public abstract class UnitAbility : EnumScriptableObject {
    [SerializeField] private GameObject skillTestPrefab;
    [SerializeField] protected EmptyGameEvent beginAbilityExecutionEvent;
    [SerializeField] protected EmptyGameEvent endAbilityExecutionEvent;

    // Result is a quality percentage from 0 - 1.
    public delegate void AbilityEffectivenessCallback(float result);
    
    [Serializable]
    public struct UnitAbilityCost {
      public int amount;
      public ExhaustibleResource resource;
    }

    public class AbilityExecutionContext {
      public EncounterActor Actor { get; set; }
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

    public abstract bool CouldExecute(AbilityExecutionContext context);
    
    /// <returns>Whether the ability is successfully executing</returns>
    public abstract bool TryExecute(AbilityExecutionContext context);

    public bool CanAfford(EncounterActor actor) {
      foreach (var abilityCost in cost) {
        if (actor.EncounterState.GetResourceAmount(abilityCost.resource) < abilityCost.amount) {
          return false;
        }
      }
      return true;
    }
    
    protected void SpendCost(EncounterActor actor) {
      foreach (var abilityCost in cost) {
        actor.EncounterState.ExpendResource(abilityCost.resource, abilityCost.amount);
      }
    }

    // TODO(P1): Determine this far more maturely, for the player and AI.
    protected void DetermineAbilityEffectiveness(EncounterActor actor, AbilityEffectivenessCallback callback) {
      if (actor.EncounterState.faction == UnitFaction.PlayerParty) {
        Instantiate(skillTestPrefab).GetComponent<SkillTestController>().Run(callback);
        return;
      }

      callback(Random.Range(0.5f, 1f));
    }
  }
}