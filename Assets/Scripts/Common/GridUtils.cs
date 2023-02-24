using System;
using System.Collections.Generic;
using Terrain;
using UnityEngine;

namespace Common {
  public class GridUtils {
    public static readonly List<Vector2Int> SurroundingNodes = new() {
        new(-1, 0),
        new(0, 1),
        new(1, 0),
        new(0, -1),
    };

    /// <summary>
    /// Executes for each adjacent tile, starting at the East tile and moving clockwise.
    /// Aggregates the results in an array indexed by Direction.
    /// </summary>
    public static void ForEachAdjacentTile(Vector2Int tile, Action<Vector2Int /* adjacentTile */> action) {
      foreach (var direction in SurroundingNodes) {
        action(tile + direction);
      }
    }
    
    public static void ForEachAdjacentTile(Vector3Int tile, Action<Vector3Int /* adjacentTile */> action) {
      foreach (var direction in SurroundingNodes) {
        action(tile + (Vector3Int)direction);
      }
    }

    public static int DistanceBetween(Vector3Int source, Vector3Int destination) {
      return Math.Abs(source.x - destination.x) + Math.Abs(source.y - destination.y);
    }

    public static Vector2[] GetFootprintPolygon(Vector2Int size) {
      return new Vector2[] {
          SceneTerrain.CellBaseWorldStatic(new Vector3Int(0, 0)),
          SceneTerrain.CellBaseWorldStatic(new Vector3Int(0, size.y)),
          SceneTerrain.CellBaseWorldStatic(new Vector3Int(size.x, size.y)),
          SceneTerrain.CellBaseWorldStatic(new Vector3Int(size.x, 0)),
      };
    }
  }
}