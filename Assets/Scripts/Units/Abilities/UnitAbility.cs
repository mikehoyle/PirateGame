using System;
using System.Collections;
using System.Text;
using Common.Animation;
using Encounters;
using Encounters.Effects;
using Encounters.Grid;
using Encounters.SkillTest;
using Events;
using FMODUnity;
using Optional;
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
    [SerializeField] protected EventReference soundOnActivate;
    [SerializeField] protected DirectionalAnimatedSprite impactAnimation;
    [SerializeField] protected float impactAnimationDelaySec;
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

    /// <returns>A coroutine enumerator for the ability, if it can be performed.</returns>
    public Option<IEnumerator> TryExecute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      if (!CouldExecute(context) || !CanAfford(context.Actor)) {
        return Option.None<IEnumerator>();
      }

      Dispatch.Encounters.AbilityExecutionStart.Raise();
      SpendCost(context.Actor);
      return Option.Some(Execute(context, () => {
        Dispatch.Encounters.AbilityExecutionEnd.Raise();
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

    // TODO(P1): Determine this far more maturely, for the player and AI.
    protected void DetermineAbilityEffectiveness(EncounterActor actor, AbilityEffectivenessCallback callback) {
      if (actor.EncounterState.faction == UnitFaction.PlayerParty) {
        if (skillTestPrefab != null) {
          Instantiate(skillTestPrefab).GetComponent<SkillTestController>().Run(callback);
          return;
        }
        callback(1f);
        return;
      }

      callback(Random.Range(0.5f, 1f));
    }

    protected Coroutine CreateImpactAnimation(Vector3Int target) {
      if (impactAnimation == null) {
        return null;
      }
      
      var impactObject = new GameObject("Impact Animation");
      impactObject.AddComponent<SpriteRenderer>();
      var animation = impactObject.AddComponent<EphemeralAnimation>();
      return animation.PlayThenDie(target, impactAnimation, "effect");
    }
  }
}