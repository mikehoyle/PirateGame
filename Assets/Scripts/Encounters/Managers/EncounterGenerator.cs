using System;
using System.Collections.Generic;
using System.Linq;
using State;
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
    [SerializeField] private EnemyUnitMetadata spawnerEnemy;
    [SerializeField] private ObstaclePrefab rockObstacle;
    [SerializeField] private RawResource lumberResource;
    
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
    public void Generate(EncounterTile encounterTile) {
      Debug.Log($"Generating encounter of DR: {encounterTile.difficultyRating}");
      GenerateTerrain(encounterTile);
      
      _availableTiles = new HashSet<Vector3Int>(encounterTile.terrain.Keys);
      GenerateObstacles(encounterTile);
      GenerateCollectables(encounterTile);
      GenerateUnits(encounterTile);
      encounterTile.isInitialized = true;
    }

    private void GenerateTerrain(EncounterTile encounterTile) {
      encounterTile.terrain = new();
      var width = _rng.Next(6, 12);
      var height = _rng.Next(6, 12);
      var poolWidth = _rng.Next(2, 4);
      var poolHeight = _rng.Next(2, 4);
      var poolX = _rng.Next(0, 8);
      var poolY = _rng.Next(0, 8);

      for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
          encounterTile.terrain.Add(new Vector3Int(x, y, 0), TerrainType.Land);
        }
      }

      for (int x = 0; x < poolWidth; x++) {
        for (int y = 0; y < poolHeight; y++) {
          encounterTile.terrain.Remove(new Vector3Int(poolX + x, poolY + y));
        }
      }
    }
    
    /// <summary>
    /// Generate units with an individual DR which sums to the DR of the encounter.
    /// </summary>
    private void GenerateUnits(EncounterTile encounterTile) {
      encounterTile.enemies = new();
      var spawnVars = new EnemySpawnVariables {
          DifficultyRating = encounterTile.difficultyRating,
      };
      
      // For now, always add spawner
      var spawner = spawnerEnemy.NewEncounter(ClaimRandomTile(spawnerEnemy.size));
      encounterTile.enemies.Add(spawner);
      
      // Now pick the remainder of enemies by weighted chance.
      var remainingDr = (float)encounterTile.difficultyRating;
      while (remainingDr > 0) {
        var currentRemainingDr = remainingDr;
        var candidates = spawnableEnemies.enemyUnits
            .Where(enemy => enemy.spawnConfig.individualDifficultyRating <= currentRemainingDr)
            .ToList();
        var totalWeight = candidates.Sum(enemy => enemy.spawnConfig.GetSpawnWeight(spawnVars));
        Debug.Log($"Currently {candidates.Count} possible candidates with a total weight of {totalWeight}");
        var choice = _rng.NextDouble() * totalWeight;
        var chosenEnemy = candidates.FirstOrDefault(enemy => {
          choice -= enemy.spawnConfig.GetSpawnWeight(spawnVars);
          return choice <= 0;
        });

        if (chosenEnemy == null) {
          Debug.LogError("Failed to choose enemy for encounter, this shouldn't be possible");
          continue;
        }

        var enemy = chosenEnemy.NewEncounter(ClaimRandomTile(chosenEnemy.size));
        encounterTile.enemies.Add(enemy);
        remainingDr -= chosenEnemy.spawnConfig.individualDifficultyRating;
      }
    }


    /// <summary>
    /// Once again, far too simple to actually be reasonable, but for now just pick
    /// a few spots and put the most basic obstacle there.
    /// </summary>
    private void GenerateObstacles(EncounterTile encounterTile) {
      encounterTile.obstacles = new();
      // Arbitrarily make 4 obstacles
      for (int i = 0; i < 4; i++) {
        var tile = ClaimRandomTile(rockObstacle.Footprint);
        foreach (var obstacle in rockObstacle.obstacles) {
          encounterTile.obstacles[tile + obstacle.Key] = obstacle.Value.RandomVariant();
        }
      }
    }
    
    /// <summary>
    /// Rework this entirely, it's just a hard-coded stub.
    /// </summary>
    private void GenerateCollectables(EncounterTile encounterTile) {
      encounterTile.collectables = new();

      // Arbitrarily make 3 collectables
      for (int i = 0; i < 3; i++) {
        var tile = ClaimRandomTile(Vector2Int.one);
        encounterTile.collectables.Add(tile, new CollectableInstance {
            contents = new() {
                // Arbitrary amount
                [lumberResource] = _rng.Next(15, 30),
            },
        });
      }
    }

    // TODO(P1): This breaks if we have no valid tile options.
    private Vector3Int ClaimRandomTile(Vector2Int size) {
      using var randomizedTiles = _availableTiles.OrderBy(_ => _rng.Next()).GetEnumerator();
      while (true) {
        randomizedTiles.MoveNext();
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
    }
    
    // TODO(P1): This breaks if we have no valid tile options.
    private Vector3Int ClaimRandomTile(AreaOfEffect aoe) {
      using var randomizedTiles = _availableTiles.OrderBy(_ => _rng.Next()).GetEnumerator();
      while (true) {
        randomizedTiles.MoveNext();
        var tile = randomizedTiles.Current;
        if (AllTilesAreFreeForArea(tile, aoe)) {
          foreach (var aoeCoord in aoe.WithTarget(tile).AffectedPoints()) {
            _availableTiles.Remove(aoeCoord);
          }
          return tile;
        }
      }
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