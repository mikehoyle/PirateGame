using System.Collections.Generic;
using System.Linq;
using State.Encounter;
using State.Unit;
using State.World;
using StaticConfig.Encounters;
using StaticConfig.RawResources;
using Units.Abilities.AOE;
using UnityEngine;
using Random = System.Random;

namespace Encounters.Managers {
  /// <summary>
  /// Encapsulates generation of an encounter. This happens at interaction time rather than
  /// upfront at map generation time, for a few reasons:
  ///  - It's not needed or seen before interaction.
  ///  - It reduces save file size and initial game load time.
  ///  - We can make dynamic choices mid-save about what is encountered.
  /// </summary>
  public class EncounterGenerator : MonoBehaviour {
    [SerializeField] private EnemyUnitTypeCollection spawnableEnemies;
    [SerializeField] private ObstaclePrefab rockObstacle;
    [SerializeField] private RawResource soulsResource;
    [SerializeField] private TerrainPrefabs terrainPrefabs;
    
    private Random _rng;
    private HashSet<Vector3Int> _availableTiles;
    
    private void Awake() {
      _rng = new Random();
    }

    /// <summary>
    /// Generate the tile for the first time. This is bound to evolve a LOT over the course of
    /// development, but for now, it's very simple.
    /// TODO(P1): oh so much more/different than this
    /// </summary>
    public void Generate(EncounterWorldTile encounterTile) {
      Debug.Log($"Generating encounter of DR: {encounterTile.difficulty}");
      GenerateTerrain(encounterTile);
      
      _availableTiles = new HashSet<Vector3Int>(encounterTile.terrain.Keys);
      GenerateCollectables(encounterTile);
      GenerateObstacles(encounterTile);
      GenerateUnits(encounterTile);
      encounterTile.isInitialized = true;
    }

    private void GenerateTerrain(EncounterWorldTile encounterTile) {
      encounterTile.terrain = new();
      /*var width = 9;
      var height = 9;

      for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
          encounterTile.terrain.Add(new Vector3Int(x, y, 0), TerrainType.Land);
        }
      }*/

      var chosenTerrain = _rng.Next(0, terrainPrefabs.Count);
      foreach (var tile in terrainPrefabs.GetTerrainMap(chosenTerrain).AffectedPoints()) {
        encounterTile.terrain.Add(tile, TerrainType.Land);
      }
    }
    
    /// <summary>
    /// Generate units with an individual DR which sums to the DR of the encounter.
    /// </summary>
    private void GenerateUnits(EncounterWorldTile encounterTile) {
      encounterTile.enemies = new();
      foreach (var chosenEnemy in spawnableEnemies.RandomEnemySpawnsForDifficulty(encounterTile.difficulty)) {
        var enemy = chosenEnemy.NewEncounter(ClaimRandomTile(chosenEnemy.size));
        encounterTile.enemies.Add(enemy);
      }
    }


    /// <summary>
    /// Once again, far too simple to actually be reasonable, but for now just pick
    /// a few spots and put the most basic obstacle there.
    /// </summary>
    private void GenerateObstacles(EncounterWorldTile encounterTile) {
      encounterTile.obstacles = new();
      // Arbitrarily make 6 obstacles
      for (int i = 0; i < 6; i++) {
        var tile = ClaimRandomTile(rockObstacle.Footprint);
        foreach (var obstacle in rockObstacle.obstacles) {
          encounterTile.obstacles[tile + obstacle.Key] = obstacle.Value.RandomVariant();
        }
      }
    }
    
    /// <summary>
    /// Rework this entirely, it's just a hard-coded stub.
    /// </summary>
    private void GenerateCollectables(EncounterWorldTile encounterTile) {
      encounterTile.collectables = new();

      using var randomizedTiles = _availableTiles
          .Where(tile => tile.y == 0)
          .OrderBy(_ => _rng.Next()).GetEnumerator();
      randomizedTiles.MoveNext();

      var crystalPosition = randomizedTiles.Current;
      _availableTiles.Remove(crystalPosition);
      encounterTile.collectables.Add(crystalPosition, new CollectableInstance {
          isPrimaryObjective = true,
          name = "Soul Key",
          contents = new() {
              // Arbitrary amount, for now
              [soulsResource] = (int)(_rng.Next(20, 30) * encounterTile.difficulty),
          },
      });
    }

    // TODO(P1): This breaks if we have no valid tile options.
    private Vector3Int ClaimRandomTile(Vector2Int size) {
      using var randomizedTiles = _availableTiles.OrderBy(_ => _rng.Next()).GetEnumerator();
      while (randomizedTiles.MoveNext()) {
        var tile = randomizedTiles.Current;
        if (AllTilesAreFreeForSize(tile, size)) {
          for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
              _availableTiles.Remove(new Vector3Int(tile.x + x, tile.y + y, tile.z));
            }
          }
          return tile;
        }
      }
      
      Debug.LogWarning("Could not find tile to claim, this shouldn't happen!");
      return new Vector3Int(99999, 99999, 0);
    }
    
    // TODO(P1): This breaks if we have no valid tile options.
    private Vector3Int ClaimRandomTile(AreaOfEffect aoe) {
      using var randomizedTiles = _availableTiles.OrderBy(_ => _rng.Next()).GetEnumerator();
      while (randomizedTiles.MoveNext()) {
        var tile = randomizedTiles.Current;
        if (AllTilesAreFreeForArea(tile, aoe)) {
          foreach (var aoeCoord in aoe.WithTarget(tile).AffectedPoints()) {
            _availableTiles.Remove(aoeCoord);
          }
          return tile;
        }
      }
      
      Debug.LogWarning("Could not find tile to claim, this shouldn't happen!");
      return new Vector3Int(99999, 99999, 0);
    }

    private bool AllTilesAreFreeForSize(Vector3Int tile, Vector2Int size) {
      for (int x = 0; x < size.x; x++) {
        for (int y = 0; y < size.y; y++) {
          if (!_availableTiles.Contains(new Vector3Int(tile.x + x, tile.y + y, tile.z))) {
            return false;
          }
        }
      }
      
      return true;
    }

    private bool AllTilesAreFreeForArea(Vector3Int tile, AreaOfEffect aoe) {
      foreach (var aoeCoord in aoe.WithTarget(tile).AffectedPoints()) {
        if (!_availableTiles.Contains(aoeCoord)) {
          return false;
        }
      }
      return true;
    }
  }
}