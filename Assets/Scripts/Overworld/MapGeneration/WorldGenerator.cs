using System;
using Common.Grid;
using State.World;
using UnityEngine;
using Zen.Hexagons;
using Random = System.Random;

namespace Overworld.MapGeneration {
  public class WorldGenerator {
    private const float OutpostBaseChance = 0f;
    private const float OutpostIncreasedChanceByLoneliness = 0.15f;
    private const int FarthestOutpostConnectionRadius = 3;

    private readonly int _worldRadius;
    private readonly Random _rng;
    private readonly HexLibrary _hexLibrary;
    private readonly HexPathGenerator _borderGenerator;

    public WorldGenerator(int worldRadius, int? seed = null) {
      _worldRadius = worldRadius;
      _rng = seed.HasValue ? new Random(seed.Value) : new Random();
      _hexLibrary = HexGridUtils.HexLibrary;
      _borderGenerator = new HexPathGenerator();
    }
    
    public WorldState GenerateWorld() {
      var world = ScriptableObject.CreateInstance<WorldState>();
      GenerateTiles(world);
      return world;
    }

    private void GenerateTiles(WorldState world) {
      GenerateOpenSea(world);
      GeneratePointsOfInterest(world);
      GenerateOutOfBoundsTiles(world);
      // Overwrite some starter tiles
      // Starter tile is always open
      world.UpdateTile(new OpenSeaWorldTile(HexOffsetCoordinates.Origin));
      GenerateBorders(world);
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
        var isTileConnectable = distance % 4 == 0; 
        world.UpdateTile(new OutOfBoundsWorldTile(
            _hexLibrary.CubeToOffsetCoordinates(new HexCubeCoordinates(distance, -distance, 0)), isTileConnectable));
        world.UpdateTile(new OutOfBoundsWorldTile(
            _hexLibrary.CubeToOffsetCoordinates(new HexCubeCoordinates(0, distance, -distance)), isTileConnectable));
        world.UpdateTile(new OutOfBoundsWorldTile(
            _hexLibrary.CubeToOffsetCoordinates(new HexCubeCoordinates(-distance, 0, distance)), isTileConnectable));
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

    private void GenerateBorders(WorldState world) {
      foreach (var coord in _hexLibrary.GetSpiralRing(new HexOffsetCoordinates(0, 0), _worldRadius)) {
        if (world.GetTile(coord).connectsToBoundaries) {
          foreach (var nearbyCell in _hexLibrary.GetSpiralRing(coord, FarthestOutpostConnectionRadius)) {
            if (world.GetTile(nearbyCell)?.connectsToBoundaries ?? false) {
              if (world.HasPath(coord, nearbyCell) || !HasUninterruptedPath(world, coord, nearbyCell)) {
                // Don't duplicate an already-made path & don't connect through out-of-bounds.
                continue;
              }
              
              var border = _borderGenerator.GeneratePath(coord, nearbyCell);

              // Exclude borders that "skirt around" boundaries, because it's crowded and unnecessarily
              // over-connected.
              var shouldInclude = true;
              foreach (var edge in border.edges) {
                if (world.GetTile(edge.borderCellOne)?.connectsToBoundaries ?? false) {
                  shouldInclude = false;
                }
                if (world.GetTile(edge.borderCellTwo)?.connectsToBoundaries ?? false) {
                  shouldInclude = false;
                }
              }

              if (shouldInclude) {
                world.outpostBorders.Add(border);
              }
            }
          }
        }
      }
    }

    private bool HasUninterruptedPath(
        WorldState world, HexOffsetCoordinates fromCell, HexOffsetCoordinates toCell) {
      // Don't allow connections through out-of-bounds tiles.
      var path = HexGridUtils.HexLibrary.GetLine(fromCell, toCell);
      if (path.Length <= 2) {
        // Adjacent cells are always uninterrupted.
        return true;
      }
      
      // Ignore first and last, iterate through the cells between.
      for (var i = 1; i < path.Length - 1; i++) {
        if (world.GetTile(path[i]) is null or OutOfBoundsWorldTile) {
          return false;
        }
      }

      return true;
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