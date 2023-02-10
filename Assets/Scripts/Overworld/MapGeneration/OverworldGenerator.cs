using System;
using System.Collections.Generic;
using State.World;
using UnityEngine;
using Random = System.Random;

namespace Overworld.MapGeneration {
  /// <summary>
  /// This captures the map generation that defines the map for the rest of the game.
  ///
  /// For now, it is just a prototype, and is set to perform some very basic operations just to have a simple map to
  /// work with.
  /// </summary>
  public class OverworldGenerator {
    private readonly int _width;
    private readonly int _height;
    private readonly Random _rng;

    public OverworldGenerator(int width, int height, int? seed) {
      _width = width;
      _height = height;
      _rng = seed.HasValue ? new Random(seed.Value) : new Random();
    }
    
    public WorldState GenerateWorld() {
      var world = new WorldState();
      GenerateTiles(world);
      return world;
    }
    
    /// <summary>
    /// Current strategy is just spiral out and place random stuff.
    /// </summary>
    private void GenerateTiles(WorldState world) {
      GenerateRandomSpiral(world);
      
      // Overwrite some starter tiles
      // Starter tile is always open ocean
      world.SetTile(0, 0, ScriptableObject.CreateInstance<OpenSeaTile>());
    }
    private void GenerateRandomSpiral(WorldState world) {
      var sideLength = 1;
      var currentSideProgression = 0;
      var countdownToLongerLength = 2;
      Vector2Int currentTile = Vector2Int.zero;
      

      LinkedList<Vector2Int> directions = new(new List<Vector2Int> {
          Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
      });
      var currentDirection = directions.First;
      
      while (true) {
        if (Math.Abs(currentTile.x) > _width / 2 || Math.Abs(currentTile.y) > _height / 2) {
          return;
        }
        world.SetTile(currentTile.x, currentTile.y, RandomTile());
        currentTile += currentDirection.Value;
        
        currentSideProgression++;

        if (currentSideProgression >= sideLength) {
          currentSideProgression = 0;
          currentDirection = currentDirection.Next ?? directions.First;
          countdownToLongerLength -= 1;
          if (countdownToLongerLength == 0) {
            sideLength++;
            countdownToLongerLength = 2;
          }
        }
      }
    }

    private WorldTile RandomTile() {
      return _rng.NextDouble() switch {
          var x when x < 0.2 => ScriptableObject.CreateInstance<EncounterTile>(),
          _ => ScriptableObject.CreateInstance<OpenSeaTile>(),
      };
    }
  }
}