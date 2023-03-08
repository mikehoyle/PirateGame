using System;
using State.World;
using UnityEngine;
using Zen.Hexagons;
using Random = System.Random;

namespace Overworld.MapGeneration {
  public class WorldGenerator {
    private const float OutpostBaseChance = 0f;
    private const float OutpostIncreasedChanceByLoneliness = 0.1f;

    private readonly int _worldRadius;
    private readonly Random _rng;
    private HexLibrary _hexLibrary;

    public WorldGenerator(int worldRadius, int? seed = null) {
      _worldRadius = worldRadius;
      _rng = seed.HasValue ? new Random(seed.Value) : new Random();
      _hexLibrary = new HexLibrary(HexType.FlatTopped, OffsetCoordinatesType.Odd, 1);
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
      GenerateOpenSea(world);
      GeneratePointsOfInterest(world);
      GenerateOutOfBoundsTiles(world);
      // Overwrite some starter tiles
      // Starter tile is always open
      world.UpdateTile(new OpenSeaWorldTile(HexOffsetCoordinates.Origin));
    }

    private void GenerateOutOfBoundsTiles(WorldState world) {
      GenerateOutOfBoundsTilesForSlices(world);
       
      // World's edge 
      foreach (var borderTile in _hexLibrary.GetSingleRing(new HexOffsetCoordinates(0, 0), _worldRadius + 1)) {
        world.UpdateTile(new OutOfBoundsWorldTile(borderTile));
      }
    }

    private void GenerateOutOfBoundsTilesForSlices(WorldState world) {
      // Creates a Y shape, slicing the world into thirds.
      var distance = 1;
      while (distance < _worldRadius) {
        world.UpdateTile(new OutOfBoundsWorldTile(
            _hexLibrary.CubeToOffsetCoordinates(new HexCubeCoordinates(distance, -distance, 0))));
        world.UpdateTile(new OutOfBoundsWorldTile(
            _hexLibrary.CubeToOffsetCoordinates(new HexCubeCoordinates(0, distance, -distance))));
        world.UpdateTile(new OutOfBoundsWorldTile(
            _hexLibrary.CubeToOffsetCoordinates(new HexCubeCoordinates(-distance, 0, distance))));
        distance++;
      }
    }

    private void GenerateOpenSea(WorldState world) {
      foreach (var borderTile in _hexLibrary.GetSpiralRing(new HexOffsetCoordinates(0, 0), _worldRadius)) {
        world.TrySetTile(new OpenSeaWorldTile(borderTile));
      }
    }

    private void GeneratePointsOfInterest(WorldState world) {
      var origin = new HexOffsetCoordinates(0, 0);
      foreach (var coord in _hexLibrary.GetSpiralRing(new HexOffsetCoordinates(0, 0), _worldRadius)) {
        if (_hexLibrary.GetDistance(coord, origin) == 1) {
          // Always surround player with outposts.
          world.UpdateTile(new EncounterWorldTile(coord));
          continue;
        }

        var closestEncounter = ClosestEncounter(world, coord);
        if (closestEncounter == 1) {
          // No adjacent encounters allowed
          continue;
        }

        var encounterChance = OutpostBaseChance + (OutpostIncreasedChanceByLoneliness * closestEncounter);
        if (_rng.NextDouble() <= encounterChance) {
          world.UpdateTile(new EncounterWorldTile(coord));
        }
      }
    }

    private int ClosestEncounter(WorldState world, HexOffsetCoordinates coordinates) {
      var closestEncounter = int.MaxValue;
      foreach (var adjacentCoord in _hexLibrary.GetSpiralRing(coordinates, 5)) {
        if (world.GetTile(adjacentCoord) is EncounterWorldTile) {
          closestEncounter = Math.Min(closestEncounter, _hexLibrary.GetDistance(coordinates, adjacentCoord));
        }
      }
      return closestEncounter;
    }
  }
}