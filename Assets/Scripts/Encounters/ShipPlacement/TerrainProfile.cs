using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encounters.ShipPlacement {
  /// <summary>
  /// An accessible representation of the outer profile of a piece of terrain.
  /// This code is absolutely terrible, but it works for now.
  /// </summary>
  public class TerrainProfile {
    public struct Overlap {
      public int Top;
      public int Bottom;
      public int Right;
      public int Left;

      /// <summary>
      /// Suggests an offset to rectify the overlap.
      /// </summary>
      public Vector3Int? SuggestedSnapOffset() {
        var currentOffset = int.MaxValue;
        var result = new Vector3Int();
        if (Top != int.MinValue && Math.Abs(Top) < currentOffset) {
          currentOffset = Math.Abs(Top);
          // plus one because we're shooting for adjacency, not overlapping edges.
          result = new Vector3Int(0, -Top - 1, 0);
        }
        
        if (Bottom != int.MinValue && Math.Abs(Bottom) < currentOffset) {
          currentOffset = Math.Abs(Bottom);
          result = new Vector3Int(0, -Bottom + 1, 0);
        }
        
        if (Right != int.MinValue && Math.Abs(Right) < currentOffset) {
          currentOffset = Math.Abs(Right);
          result = new Vector3Int(-Right - 1, 0, 0);
        }
        
        if (Left != int.MinValue && Math.Abs(Left) < currentOffset) {
          currentOffset = Math.Abs(Right);
          result = new Vector3Int(-Left + 1, 0, 0);
        }

        return currentOffset == int.MaxValue ? null : result;
      }

      public override string ToString() {
        return $"Top: {Top}, Bottom: {Bottom}, Right: {Right}, Left: {Left}";
      }
    }
    
    public readonly Dictionary<int, int> TopEdge = new();
    public readonly Dictionary<int, int> BottomEdge = new();
    public readonly Dictionary<int, int> RightEdge = new();
    public readonly Dictionary<int, int> LeftEdge = new();

    public static TerrainProfile BuildFrom(IEnumerable<Vector3Int> coords) {
      var result = new TerrainProfile();
      foreach (var coord in coords) {
        if (!result.TopEdge.TryAdd(coord.x, coord.y)) {
          result.TopEdge[coord.x] = Math.Max(result.TopEdge[coord.x], coord.y);
        }
        if (!result.BottomEdge.TryAdd(coord.x, coord.y)) {
          result.BottomEdge[coord.x] = Math.Min(result.BottomEdge[coord.x], coord.y);
        }
        if (!result.RightEdge.TryAdd(coord.y, coord.x)) {
          result.RightEdge[coord.y] = Math.Max(result.RightEdge[coord.y], coord.x);
        }
        if (!result.LeftEdge.TryAdd(coord.y, coord.x)) {
          result.LeftEdge[coord.y] = Math.Min(result.LeftEdge[coord.y], coord.x);
        }
      }
      
      return result;
    }

    public Overlap CalculateEdgeOverlap(Vector3Int offset, TerrainProfile otherProfile) {
      var result = new Overlap();

      var maxTopOverlap = int.MinValue;
      foreach (var x in TopEdge.Keys) {
        if (otherProfile.BottomEdge.TryGetValue(x + offset.x, out var y)) {
          maxTopOverlap = Math.Max(maxTopOverlap, (TopEdge[x] + offset.y) + y);
        }
      }
      result.Top = maxTopOverlap;
      
      var maxBotOverlap = int.MaxValue;
      foreach (var x in BottomEdge.Keys) {
        if (otherProfile.TopEdge.TryGetValue(x + offset.x, out var y)) {
          maxBotOverlap = Math.Min(maxBotOverlap, (BottomEdge[x] + offset.y) - y);
        }
      }
      result.Bottom = maxBotOverlap;
      
      var maxRightOverlap = int.MinValue;
      foreach (var y in RightEdge.Keys) {
        if (otherProfile.LeftEdge.TryGetValue(y + offset.y, out var x)) {
          maxRightOverlap = Math.Max(maxRightOverlap, (RightEdge[y] + offset.x) + x);
        }
      }
      result.Right = maxRightOverlap;
      
      var maxLeftOverlap = int.MaxValue;
      foreach (var y in LeftEdge.Keys) {
        if (otherProfile.RightEdge.TryGetValue(y + offset.y, out var x)) {
          maxLeftOverlap = Math.Min(maxLeftOverlap, (LeftEdge[y] + offset.x) - x);
        }
      }
      result.Left = maxLeftOverlap;

      return result;
    }

    public override string ToString() {
      return $"Top: {string.Join(", ", TopEdge)}\n" +
          $"Bottom: {string.Join(", ", BottomEdge)}\n" +
          $"Left: {string.Join(", ", LeftEdge)}\n" +
          $"Right: {string.Join(", ", RightEdge)}\n";
    }
  }
}