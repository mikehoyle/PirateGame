using System;
using UnityEngine;
using static State.Unit.FacingDirection;

namespace State.Unit {
  public enum FacingDirection {
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest,
  }

  public static class FacingUtilities {
    public static Vector2Int ToUnitVector(this FacingDirection facingDirection) {
      return facingDirection switch {
          NorthEast => new Vector2Int(1, 0),
          NorthWest => new Vector2Int(0, 1),
          SouthEast => new Vector2Int(0, -1),
          // Arbitrarily default to SW
          _ => new Vector2Int(-1, 0),
      };
    }

    public static FacingDirection DirectionBetween(Vector2Int source, Vector2Int target) {
      var directionVector = target - source;
      // Strategy: choose the "dominant" difference in direction, and use that to determine facing. If the x and y
      // diff are equal, it's a diagonal (wash), so prefer the more negative of the two (i.e. facing camera).
      bool preferX = directionVector switch {
          { x: var x, y: var y } when Math.Abs(x) > Math.Abs(y) => true,
          { x: var x, y: var y } when Math.Abs(x) < Math.Abs(y) => false,
          { x: var x, y: var y } when x < y => true,
          _ => false,
      };

      if (preferX) {
        if (directionVector.x > 0) {
          return NorthEast;
        }
        return SouthWest;
      }
      if (directionVector.y > 0) {
        return NorthWest;
      }
      return SouthEast;
    }

    /// <summary>
    /// More liberally determine if a target is in the source's FOV. There is overlap at the diagonals.
    /// Also note that any location is always considered to be in its own FOV.
    /// </summary>
    public static bool IsInFov(this FacingDirection facingDirection, Vector3Int source, Vector3Int target) {
      Debug.Log($"Determining FOV range for direction {facingDirection}");
      var directionVector = (Vector2Int)target - (Vector2Int)source;

      return facingDirection switch {
          NorthEast => directionVector.x >= 0 && Math.Abs(directionVector.y) <= directionVector.x,
          NorthWest => directionVector.y >= 0 && Math.Abs(directionVector.x) <= directionVector.y,
          SouthEast => directionVector.y <= 0 && Math.Abs(directionVector.x) <= Math.Abs(directionVector.y),
          // Again, arbitrarily default to SW
          _ => directionVector.x <= 0 && Math.Abs(directionVector.y) <= Math.Abs(directionVector.x), 
      };
    }
  }
}