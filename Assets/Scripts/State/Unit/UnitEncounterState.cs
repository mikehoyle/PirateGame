using System;
using System.Collections.Generic;
using Common.Animation;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace State.Unit {
  [Serializable]
  public class UnitEncounterState {
    public UnitMetadata metadata;
    public ExhaustibleResourceTracker[] resources;
    public Vector3Int position;
    public UnitFaction faction;
    public FacingDirection facingDirection;
    
    
    private Dictionary<UnitAbility, int> _abilityExecutionCounts;

    public UnitEncounterState() {
      _abilityExecutionCounts = new();
    }
    
    public void NewRound(Func<Stat, int> statGetter) {
      foreach (var resource in resources) {
        resource.NewRound(statGetter);
      }
    }

    public void NewEncounter() {
      _abilityExecutionCounts.Clear();
      foreach (var resource in resources) {
        resource.Reset();
      }
    }

    public void ExpendResource(ExhaustibleResource resource, int amount) {
      if (TryGetResourceTracker(resource, out var tracker)) {
        Debug.Log($"Reducing {tracker.exhaustibleResource.displayName} by {amount}");
        tracker.Expend(amount);
        return;
      }

      Debug.LogWarning($"Attempted to expend not-present resource: {resource}");
    }

    public void RegisterAbilityExecution(UnitAbility ability) {
      _abilityExecutionCounts.TryAdd(ability, 0);
      _abilityExecutionCounts[ability] += 1;
    }
    
    public int GetExecutionCount(UnitAbility ability) {
      return _abilityExecutionCounts.GetValueOrDefault(ability);
    }

    public int GetResourceAmount(ExhaustibleResource resource) {
      if (TryGetResourceTracker(resource, out var tracker)) {
        return tracker.current;
      }
      
      Debug.LogWarning($"Cannot get resource {resource.displayName}, unit does not have it");
      return 0;
    }

    public bool TryGetResourceTracker(ExhaustibleResource resource, out ExhaustibleResourceTracker tracker) {
      foreach (var resourceTracker in resources) {
        if (resourceTracker.exhaustibleResource == resource) {
          tracker = resourceTracker;
          return true;
        }
      }
      tracker = null;
      return false;
    }
    
    /// <summary>
    /// All the tiles a unit is on. For most (size=1) units, this will just be the tile they stand on.
    /// </summary>
    public List<Vector3Int> OccupiedTiles() {
      var result = new List<Vector3Int>();
      for (int x = 0; x < metadata.size.x; x++) {
        for (int y = 0; y < metadata.size.y; y++) {
          result.Add(position + new Vector3Int(x, y));
        }
      }
      
      return result;
    }

    public UnitFaction OpposingFaction() {
      return faction == UnitFaction.PlayerParty ? UnitFaction.Enemy : UnitFaction.PlayerParty;
    }
  }
}