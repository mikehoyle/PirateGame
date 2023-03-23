using System;
using System.Collections.Generic;
using UnityEngine;
using Zen.Hexagons;

namespace Common.Grid {
  public static class GridUtils {
    public const float CellWidthInWorldUnits = 1;
    public const float CellHeightInWorldUnits = 0.5f;
    private const float CellHalfWidth = CellWidthInWorldUnits / 2;
    private const float CellHalfHeight = CellHeightInWorldUnits / 2;
    private const float ZHeight = 1.5f;

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

    /// <summary>
    /// From the base (0,0), moving clockwise.
    /// </summary>
    public static Vector2[] GetFootprintPolygon(Vector2Int size) {
      return new Vector2[] {
          CellBaseWorld(new Vector3Int(0, 0)),
          CellBaseWorld(new Vector3Int(0, size.y)),
          CellBaseWorld(new Vector3Int(size.x, size.y)),
          CellBaseWorld(new Vector3Int(size.x, 0)),
      };
    }
    
    /// <summary>
    /// Because this is just a simple calculation, there's no reason we can't calculate it on our own
    /// without needing access to the game object 
    /// </summary>
    public static Vector3 CellBaseWorld(Vector3Int coord) {
      return new Vector3(
          (coord.x - coord.y) * CellHalfWidth,
          ((coord.x + coord.y) * CellHalfHeight) + (coord.z * ZHeight * CellHalfHeight),
          coord.z * ZHeight);
    }

    public static Vector3 CellCenterWorld(Vector3Int coord) {
      return CellBaseWorld(coord) + new Vector3(0, CellHeightInWorldUnits / 2, 0);
    }
    
    public static Vector3 CellAnchorWorld(Vector3Int coord) {
      return CellBaseWorld(coord) + new Vector3(0, CellHeightInWorldUnits, 0);
    }
    
    /// <summary>
    /// World -> Cell transformation. But notably does not confine to integer, so also includes
    /// data on where in the cell the coordinate lies. Also ignores any Z height.
    /// </summary>
    public static Vector3 WorldToCell(Vector3 worldCoord) {
      return new Vector3(
          ((worldCoord.x / CellHalfWidth) + (worldCoord.y / CellHalfHeight)) / 2,
          ((worldCoord.y / CellHalfHeight) - (worldCoord.x / CellHalfWidth)) / 2,
          worldCoord.z);
    }
  }
}