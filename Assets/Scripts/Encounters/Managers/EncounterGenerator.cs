using System;
using System.Collections.Generic;
using System.Linq;
using State;
using State.Encounter;
using State.Unit;
using State.World;
using StaticConfig.Encounters;
using StaticConfig.RawResources;
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
    [SerializeField] private EnemyUnitMetadata basicEnemy;
    [SerializeField] private EnemyUnitMetadata spawnerEnemy;
    [SerializeField] private ObstaclePrefab rockObstacle;
    [SerializeField] private RawResource lumberResource;
    
    private Random _rng;
    
    private void Awake() {
      _rng = new Random();
    }

    /// <summary>
    /// Generate the tile for the first time. This is bound to evolve a LOT over the course of
    /// development, but for now, it's very simple.
    /// TODO(P1): oh so much more/different than this
    /// </summary>
    public void Generate(EncounterTile encounterTile) {
      GenerateTerrain(encounterTile);
      var claimedTiles = GenerateObstacles(encounterTile);
      claimedTiles = GenerateCollectables(encounterTile, claimedTiles);
      GenerateUnits(encounterTile, claimedTiles);
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
    /// This method is definitely un-fun but for now, just generate a number of units similar
    /// to what the player has, and hard-code their stats.
    ///
    /// Returns the tiles that have been claimed by units
    /// </summary>
    private HashSet<Vector3Int> GenerateUnits(EncounterTile encounterTile, HashSet<Vector3Int> claimedTiles) {
      encounterTile.enemies = new();
      var playerUnits = GameState.State.player.roster.Count;
      var numUnits = _rng.Next(
          Math.Max(playerUnits - 1, 1), playerUnits + 2);

      using var randomPositions = encounterTile.terrain.Keys.OrderBy(_ => _rng.Next()).GetEnumerator();
      randomPositions.MoveNext();
      var spawner = spawnerEnemy.NewEncounter(randomPositions.Current);
      encounterTile.enemies.Add(spawner);
      claimedTiles.UnionWith(spawner.OccupiedTiles());

      int unitsCreated = 0;
      while (unitsCreated < numUnits) {
        randomPositions.MoveNext();
        if (claimedTiles.Contains(randomPositions.Current)) {
          continue;
        }
        
        var enemy = basicEnemy.NewEncounter(randomPositions.Current);
        encounterTile.enemies.Add(enemy);
        claimedTiles.UnionWith(enemy.OccupiedTiles());
        unitsCreated++;
      }

      return claimedTiles;
    }
    
    
    /// <summary>
    /// Once again, far too simple to actually be reasonable, but for now just pick
    /// a few spots and put the most basic obstacle there.
    /// </summary>
    /// <returns>The tiles that have been claimed by obstacles</returns>
    private HashSet<Vector3Int> GenerateObstacles(EncounterTile encounterTile) {
      encounterTile.obstacles = new();
      var claimedTiles = new HashSet<Vector3Int>();
      var obstaclesToCreate = 4;
      using var randomPositions = encounterTile.terrain.Keys.OrderBy(_ => _rng.Next()).GetEnumerator();
      
      while (obstaclesToCreate > 0) {
        randomPositions.MoveNext();
        // This method is inefficient and could theoretically exhaust all the possible terrain points and
        // error out. Eventually rework it (along with the rest of this class).
        var positionIsInvalid = false;
        foreach (var obstacleFootprintCoord in
            rockObstacle.Footprint.WithTarget(randomPositions.Current).AffectedPoints()) {
          if (claimedTiles.Contains(obstacleFootprintCoord)) {
            positionIsInvalid = true;
            break;
          }
        }
        if (positionIsInvalid) {
          continue;
        }

        foreach (var obstacle in rockObstacle.obstacles) {
          encounterTile.obstacles[randomPositions.Current + obstacle.Key] = obstacle.Value.RandomVariant();
        }
         
        claimedTiles.UnionWith(rockObstacle.Footprint.WithTarget(randomPositions.Current).AffectedPoints());
        obstaclesToCreate--;
      }

      return claimedTiles;
    }
    
    /// <summary>
    /// Rework this entirely, it's just a hard-coded stub.
    /// </summary>
    private HashSet<Vector3Int> GenerateCollectables(EncounterTile encounterTile, HashSet<Vector3Int> claimedTiles) {
      encounterTile.collectables = new();
      var collectablesToCreate = 3;
      using var randomPositions = encounterTile.terrain.Keys.OrderBy(_ => _rng.Next()).GetEnumerator();

      while (collectablesToCreate > 0) {
        randomPositions.MoveNext();
        if (claimedTiles.Contains(randomPositions.Current)) {
          continue;
        }

        var collectable = new CollectableInstance {
            contents = new() {
                [lumberResource] = _rng.Next(15, 30),
            },
        };
        encounterTile.collectables.Add(randomPositions.Current, collectable);
        collectablesToCreate--;
      }

      return claimedTiles;
    }
  }
}