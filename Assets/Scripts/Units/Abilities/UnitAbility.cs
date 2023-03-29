using System;
using System.Collections;
using System.Text;
using Encounters;
using Encounters.Effects;
using Encounters.Grid;
using Encounters.SkillTest;
using Events;
using Optional;
using RuntimeVars.Encounters;
using State.Unit;
using StaticConfig.Units;
using Terrain;
using Units.Abilities.FX;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units.Abilities {
  /// <summary>
  /// Encapsulates an actionable capability of a unit.
  /// Expand on this greatly.
  /// </summary>
  public abstract class UnitAbility : ScriptableObject {
    [SerializeField] private GameObject skillTestPrefab;
    [SerializeField] protected AbilityCastEffect fx;
    public int usesPerEncounter;
    [SerializeField] protected bool canStillMoveAfter;
    // Optional
    [SerializeReference, SerializeReferenceButton] public StatusEffect incurredEffect;
    public string descriptionShort;

    // Result is a quality percentage from 0 - 1.
    public delegate void AbilityEffectivenessCallback(float result);
    public delegate void AbilityExecutionCompleteCallback();

    [Serializable]
    public struct UnitAbilityCost {
      public int amount;
      public ExhaustibleResource resource;
    }

    public class AbilityExecutionContext {
      public EncounterActor Actor { get; set; }
      public Vector3Int Source { get; set; }
      public Vector3Int TargetedTile { get; set; }
      public SceneTerrain Terrain { get; set; }
      public GridIndicators Indicators { get; set; }
    }

    public string displayString;
    public UnitAbilityCost[] cost;

    public virtual void OnSelected(EncounterActor actor, GridIndicators indicators, Vector3Int source) { }
    
    public virtual void ShowIndicator(
        EncounterActor actor,
        Vector3Int source, 
        Vector3Int hoveredTile,
        GridIndicators indicators) { }

    public abstract bool CouldExecute(AbilityExecutionContext context);

    /// <returns>A coroutine enumerator for the ability, if it can be performed.</returns>
    public Option<IEnumerator> TryExecute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      if (!CouldExecute(context) || !CanAfford(context.Actor)) {
        return Option.None<IEnumerator>();
      }

      Dispatch.Encounters.AbilityExecutionStart.Raise(context.Actor, this);
      SpendCost(context.Actor);
      CurrentSelection.Instance.Clear();
      return Option.Some(Execute(context, () => {
        if (context.Actor is PlayerUnitController playerUnit) {
          CurrentSelection.Instance.SelectUnit(playerUnit);
          CurrentSelection.Instance.SelectAbility(playerUnit, this);
        }
        Dispatch.Encounters.AbilityExecutionEnd.Raise(context.Actor, this);
        callback();
      }));
    }
    
    protected abstract IEnumerator Execute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback); 

    public bool CanAfford(EncounterActor actor) {
      if (usesPerEncounter > 0 && actor.EncounterState.GetExecutionCount(this) >= usesPerEncounter) {
        return false;
      }
      
      foreach (var abilityCost in cost) {
        if (actor.EncounterState.GetResourceAmount(abilityCost.resource) < abilityCost.amount) {
          return false;
        }
      }
      return true;
    }

    public int GetRemainingUses(EncounterActor actor) {
      return Math.Max(0, usesPerEncounter - actor.EncounterState.GetExecutionCount(this));
    }

    public string CostString() {
      var result = new StringBuilder();
      for (int i = 0; i < cost.Length; i++) {
        result.Append($"{cost[i].amount} {cost[i].resource.displayName}");
        if (i != cost.Length - 1) {
          result.Append(", ");
        }
      }
      return result.ToString();
    }
    
    protected void SpendCost(EncounterActor actor) {
      actor.EncounterState.RegisterAbilityExecution(this);
      foreach (var abilityCost in cost) {
        actor.EncounterState.ExpendResource(abilityCost.resource, abilityCost.amount);
      }

      if (!canStillMoveAfter) {
        actor.EncounterState.ExpendResource(ExhaustibleResources.Instance.mp, int.MaxValue);
      }
    }
    
    protected void DetermineAbilityEffectiveness(EncounterActor actor, AbilityEffectivenessCallback callback) {
      // Right now, this is a non-factor. Considering dropping the idea of skill-tests entirely.
      callback(1f);
    }
  }
}