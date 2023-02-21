using System;
using System.Collections.Generic;
using StaticConfig.Units;
using UnityEngine;

namespace State.Unit {
  [Serializable]
  public class UnitEncounterState {
    public UnitMetadata metadata;
    public ExhaustibleResourceTracker[] resources;
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
  }
}