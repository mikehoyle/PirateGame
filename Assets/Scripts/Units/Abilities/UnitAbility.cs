using System;
using System.Collections;
using System.Text;
using Encounters;
using Encounters.Effects;
using Encounters.Grid;
using Encounters.SkillTest;
using FMODUnity;
using Optional;
using RuntimeVars.Encounters.Events;
using State.Unit;
using StaticConfig.Units;
using Terrain;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units.Abilities {
  /// <summary>
  /// Encapsulates an actionable capability of a unit.
  /// Expand on this greatly.
  /// </summary>
  public abstract class UnitAbility : ScriptableObject {
    [SerializeField] private GameObject skillTestPrefab;
    [SerializeField] protected EncounterEvents encounterEvents;
    [SerializeField] protected EventReference soundOnActivate;
    // Optional
    [SerializeReference, SerializeReferenceButton] public StatusEffect incurredEffect;

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
      public GameObject TargetedObject { get; set; }
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
        GameObject hoveredObject, 
        Vector3Int hoveredTile,
        GridIndicators indicators) { }

    public abstract bool CouldExecute(AbilityExecutionContext context);

    /// <returns>A coroutine enumrator for the ability, if it can be performed.</returns>
    public Option<IEnumerator> TryExecute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      if (!CouldExecute(context) || !CanAfford(context.Actor)) {
        return Option.None<IEnumerator>();
      }

      encounterEvents.abilityExecutionStart.Raise();
      SpendCost(context.Actor);
      return Option.Some(Execute(context, () => {
        encounterEvents.abilityExecutionEnd.Raise();
        callback();
      }));
    }
    
    protected void PlaySound() {
      if (!soundOnActivate.IsNull) {
        RuntimeManager.PlayOneShot(soundOnActivate);
      }
    }
    
    protected abstract IEnumerator Execute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback); 

    public bool CanAfford(EncounterActor actor) {
      foreach (var abilityCost in cost) {
        if (actor.EncounterState.GetResourceAmount(abilityCost.resource) < abilityCost.amount) {
          return false;
        }
      }
      return true;
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