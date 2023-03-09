using System;
using System.Collections.Generic;
using UnityEngine;
using Zen.Hexagons;
using Zen.Hexagons.ExtensionMethods;

namespace Common.Grid {
  public static class HexGridUtils {
    private const float FloatComparisonError = 0.001f;
    private static readonly Vector3 HexCellSize = new(0.8659766f, 1, 1);
    public static readonly HexLibrary HexLibrary = new(HexType.PointyTopped, OffsetCoordinatesType.Odd, 1);

    public static void ForEachAdjacentTileInRange(Vector2Int tile, int range, Action<HexOffsetCoordinates> action) {
      foreach (var adjacentCell in HexLibrary.GetSpiralRing(HexOffsetCoordinates.From((Vector3Int)tile), range)) {
        action(adjacentCell);
      }
    }

    public static List<Vector3> HexTileVertices(Vector3 worldCenter) {
      var halfWidth = HexCellSize.x / 2;
      var quarterHeight= HexCellSize.y / 4;
      // Starting NW corner, moving clockwise.
      return new List<Vector3> {
          worldCenter + new Vector3(-halfWidth, quarterHeight, 0),
          worldCenter + new Vector3(0, quarterHeight * 2, 0),
          worldCenter + new Vector3(halfWidth, quarterHeight, 0),
          worldCenter + new Vector3(halfWidth, -quarterHeight, 0),
          worldCenter + new Vector3(0, -quarterHeight * 2, 0),
          worldCenter + new Vector3(-halfWidth, -quarterHeight, 0),
      };
    }

    public static bool AboutEquals(this Vector3 value1, Vector3 value2) {
      return Math.Abs(value1.x - value2.x) < FloatComparisonError
          && Math.Abs(value1.y - value2.y) < FloatComparisonError
          && Math.Abs(value1.z - value2.z) < FloatComparisonError;
    }
  }
}