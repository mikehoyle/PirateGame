using System.Collections.Generic;
using System.Linq;
using Encounters.Enemies;
using Events;
using Terrain;
using UnityEngine;

namespace Encounters.Effects {
  public class AppliedSpawnerStatusEffect : AppliedPerRoundStatusEffect {
    // A square around what we expect to be a 2x2 spawner.
    private static readonly List<Vector3Int> ValidSpawnLocations = new List<Vector3Int> {
        new(-1, 0, 0),
        new(-1, 1, 0),
        new(-1, 2, 0),
        new(0, 2, 0),
        new(1, 2, 0),
        new(2, 2, 0),
        new(2, 1, 0),
        new(2, 0, 0),
        new(2, -1, 0),
        new(1, -1, 0),
        new(0, -1, 0),
        new(-1, -1, 0),
    };
    
    private SceneTerrain _terrain;

    private void Awake() {
      _terrain = SceneTerrain.Get();
    }

    protected override void EnactEffect() {
      var sourceEffect = (SpawnerStatusEffect)_sourceEffect;
      if (_terrain == null) {
        Debug.LogWarning("Cannot spawn units, no known terrain");
      }

      var validSpawnPositions = GetValidSpawnPositions(_terrain, _victim.Position);
      var numUnitsToSpawn = Random.Range(sourceEffect.minUnitsToSpawn, sourceEffect.maxUnitsToSpawn + 1);
      for (int i = 0; i < numUnitsToSpawn; i++) {
        if (validSpawnPositions.Count == 0) {
          Debug.LogWarning("No valid spawn positions in which to spawn units");
          return;
        }
        
        var unit = Instantiate(sourceEffect.enemyUnitPrefab).GetComponent<EnemyUnitController>();
        unit.Init(sourceEffect.unitToSpawn.NewEncounter(_victim.Position + validSpawnPositions.Pop()));
        Dispatch.Encounters.UnitAddedMidEncounter.Raise(unit);
      }
    }
    
    private Stack<Vector3Int> GetValidSpawnPositions(SceneTerrain terrain, Vector3Int origin) {
      return new(ValidSpawnLocations
          .Where(offset => terrain.IsTileEligibleForUnitOccupation(origin + offset))
          .OrderBy(_ => Random.Range(0f, 1f)));
    }
  }
}