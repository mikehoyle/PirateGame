using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common {
  public class IsometricGridUtils {
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
  }
}