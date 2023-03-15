using System.Collections.Generic;
using Encounters.Effects;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace State.Unit {
  public abstract class UnitMetadata : ScriptableObject {
    private const int NumActionPoints = 2;
    
    [SerializeField] protected ExhaustibleResources exhaustibleResources;
    [SerializeReference, SerializeReferenceButton]
    public StatusEffect[] passiveEffects;
    public StatTracker[] stats;
    public GameObject prefab;
    public Vector2Int size = Vector2Int.one;
    public bool isRevivable = true;

    public abstract List<UnitAbility> GetAbilities();

    public abstract string GetName();
    public abstract int GetStartingHp();
    public abstract int GetMovementRange();
    
    public int GetActionPoints() {
      return NumActionPoints;
    }

    public ExhaustibleResourceTracker[] GetEncounterTrackers() {
      return new[] {
          // TODO(P1): Don't always refresh HP fully.
          ExhaustibleResourceTracker.NewTracker(exhaustibleResources.hp, GetStartingHp()),
          ExhaustibleResourceTracker.NewTracker(exhaustibleResources.mp, GetMovementRange()),
          ExhaustibleResourceTracker.NewTracker(exhaustibleResources.ap, GetActionPoints()),
      };
    }
    
    public bool TryGetStatTracker(Stat stat, out StatTracker tracker) {
      foreach (var statTracker in stats) {
        if (statTracker.stat == stat) {
          tracker = statTracker;
          return true;
        }
      }
      tracker = null;
      return false;
    }

    public int GetStat(Stat stat) {
      if (TryGetStatTracker(stat, out var tracker)) {
        return tracker.current;
      }
      
      Debug.LogWarning($"Cannot get resource {stat.displayName}, unit does not have it");
      return 0;
    }
  }
}