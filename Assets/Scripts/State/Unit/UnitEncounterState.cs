using StaticConfig.Units;
using UnityEngine;

namespace State.Unit {
  [CreateAssetMenu(menuName = "State/UnitEncounterState")]
  public class UnitEncounterState : ScriptableObject {
    public ExhaustibleResourceTracker[] resources;
    public StatTracker[] stats;
    public Vector3Int position;
    public UnitFaction faction;
    public FacingDirection facingDirection;

    public void NewRound() {
      foreach (var resource in resources) {
        resource.NewRound();
      }
    }

    public void NewEncounter() {
      foreach (var resource in resources) {
        resource.Reset();
      }
    }
    
    public void NewEncounter(Vector3Int startingPosition) {
      NewEncounter();
      position = startingPosition;
    }

    public void ExpendResource(ExhaustibleResource resource, int amount) {
      if (TryGetResourceTracker(resource, out var tracker)) {
        Debug.Log($"Reducing {tracker.exhaustibleResource.displayName} by {amount}");
        tracker.Expend(amount);
        return;
      }

      Debug.LogWarning($"Attempted to expend not-present resource: {resource}");
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