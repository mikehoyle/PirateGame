using System.Collections.Generic;
using System.Linq;
using Encounters.Enemies;
using State.Unit;
using Terrain;
using UnityEngine;

namespace Encounters.Effects {
  [CreateAssetMenu(menuName = "Effects/SpawnerStatusEffect")]
  public class SpawnerStatusEffect : PerRoundStatusEffect {
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

    // TODO(P1): This all will need to be reworked to work with difficulty ratings etc.
    [SerializeField] private int minUnitsToSpawn;
    [SerializeField] private int maxUnitsToSpawn;
    [SerializeField] private EnemyUnitMetadata unitToSpawn;
    [SerializeField] private GameObject enemyUnitPrefab;

    protected override void EnactEffect(EncounterActor victim) {
      base.EnactEffect(victim);

      var terrain = SceneTerrain.Get();
      if (terrain == null) {
        Debug.LogWarning("Cannot spawn units, no known terrain");
      }

      var validSpawnPositions = GetValidSpawnPositions(terrain, victim.Position);
      var numUnitsToSpawn = Random.Range(minUnitsToSpawn, maxUnitsToSpawn + 1);
      for (int i = 0; i < numUnitsToSpawn; i++) {
        if (validSpawnPositions.Count == 0) {
          Debug.LogWarning("No valid spawn positions in which to spawn units");
          return;
        }
        
        var unit = Instantiate(enemyUnitPrefab).GetComponent<EnemyUnitController>();
        unit.Init(unitToSpawn.NewEncounter(victim.Position + validSpawnPositions.Pop()));
        encounterEvents.unitAddedMidEncounter.Raise(unit);
      }
    }
    private Stack<Vector3Int> GetValidSpawnPositions(SceneTerrain terrain, Vector3Int origin) {
      return new(ValidSpawnLocations
          .Where(offset => terrain.IsTileEligibleForUnitOccupation(origin + offset))
          .OrderBy(_ => Random.Range(0f, 1f)));
    }
  }
}