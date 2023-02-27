using System;
using System.Collections.Generic;
using State.World;
using UnityEngine;
using Common;


using Random = System.Random;
using Zen.Hexagons;

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
    private Vector2Int heartPositionNW;
    private Vector2Int heartPositionNE;
    private Vector2Int heartPositionSW;
    private HexLibrary _hexLibrary;
    public OverworldGenerator(int width, int height, int? seed) {
      _width = width;
      _height = height;
      _rng = seed.HasValue ? new Random(seed.Value) : new Random();
      _hexLibrary = new HexLibrary(HexType.FlatTopped, OffsetCoordinatesType.Odd, width);
    }
    
    public WorldState GenerateWorld() {
      var world = ScriptableObject.CreateInstance<WorldState>();
      GenerateTiles(world);
      return world;
    }
    
    /// <summary>
    /// Current strategy is just spiral out and place random stuff.
    /// </summary>
    private void GenerateTiles(WorldState world) {
      GenerateRandomSpiral(world);
      GenerateHeart(world);
      // Overwrite some starter tiles
      // Starter tile is always open ocean
      world.SetTile(0, 0, ScriptableObject.CreateInstance<OpenSeaTile>());
    }

    private void GenerateHeart(WorldState world) {
      // Padding so the heart doesn't generate on the outer rim
      //int outerPadding = 4;

      //int heartPosY;
      //int heartPosX = _rng.Next((_width * -1 / 2) + outerPadding, (_width / 2) - outerPadding);

      //if(heartPosX < -20 && heartPosX > 20) { // If it's in an outer range for x, use any y
      //  heartPosY = _rng.Next((_height * -1 / 2) + outerPadding, (_height / 2) - outerPadding);
      //} else { // If -20 < x < 20 , pick an outer range for y
      //  heartPosY = _rng.Next(1, 3) == 1 ? _rng.Next((_height * -1 / 2) + outerPadding, -20) : _rng.Next(20, _height / 2 - outerPadding);
      //}
      heartPositionNW = new Vector2Int(_width / 2 * -1, _height / 2);
      heartPositionNE = new Vector2Int(_width / 2, _height / 2);
      heartPositionSW = new Vector2Int(_width / 2 * -1, _height / 2 * -1);
      world.SetTile(heartPositionNW.x, heartPositionNW.y, ScriptableObject.CreateInstance<HeartTile>());
      world.SetTile(heartPositionNE.x, heartPositionNE.y, ScriptableObject.CreateInstance<HeartTile>());
      world.SetTile(heartPositionSW.x, heartPositionSW.y, ScriptableObject.CreateInstance<HeartTile>());


      Debug.Log("HEART POS: " + heartPositionNW);
      Debug.Log("HEART POS: " + heartPositionNE);
      Debug.Log("HEART POS: " + heartPositionSW);
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

        // Skips setting tile when at position of heart or player
        if (!(currentTile.x == 0 && currentTile.y == 0) && !(currentTile.x == heartPositionNE.x && currentTile.y == heartPositionNE.y) &&
          !(currentTile.x == heartPositionNW.x && currentTile.y == heartPositionNW.y) && !(currentTile.x == heartPositionSW.x && currentTile.y == heartPositionSW.y)) {
          world.SetTile(currentTile.x, currentTile.y, RandomTile());
          var currentWorldTile = world.GetTile(currentTile.x, currentTile.y);
          if(currentWorldTile.TileType == WorldTile.Type.Encounter) {
            var nodeDistanceNW = _hexLibrary.GetDistance(new HexOffsetCoordinates(currentTile.x, currentTile.y), new HexOffsetCoordinates(heartPositionNW.x, heartPositionNW.y));
            var nodeDistanceNE = _hexLibrary.GetDistance(new HexOffsetCoordinates(currentTile.x, currentTile.y), new HexOffsetCoordinates(heartPositionNE.x, heartPositionNE.y));
            var nodeDistanceSW = _hexLibrary.GetDistance(new HexOffsetCoordinates(currentTile.x, currentTile.y), new HexOffsetCoordinates(heartPositionSW.x, heartPositionSW.y));
          }
        }

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