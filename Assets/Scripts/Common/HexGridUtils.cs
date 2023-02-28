using State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common {
  public class HexGridUtils {

    public static int HexDistance(int x1, int y1, int x2, int y2) {

      int xd = x1 - x2;
      int yd = y1 - ((x1 + (x1 & 1)) / 2) - (y2 - (x2 + (x2 & 1)) / 2);
      int dist = (Mathf.Abs(xd) + Mathf.Abs(xd + yd) + Mathf.Abs(yd)) / 2;
      return dist;
    }

    public static int HexDistanceOne(int x1, int y1, int x2, int y2) {

      if (x1 == x2) {
        return Math.Abs(y2 - y1);
      } else if(y1 == y2) {
        return Math.Abs(x2 - x1);
      } else {
        var dx = Math.Abs(x2 - x1);
        var dy = Math.Abs(y2 - y1);
        if (y1 < y2) {
          return dx + dy - (int)Math.Ceiling((double)(dx / 2));
        } else {
          return dx + dy - (int)Math.Floor((double)dx / 2);
        }
      }

    
    }





    public static void ForEachAdjacentTileNotInVision(Vector2Int tile, Action<Vector2Int> action) {
      var visionRange = GameState.State.player.visionRange;
      int[] adjRows;
      int[] adjCols;
      for (int range = 0; range < visionRange; range++) {
        if ((Math.Abs(tile.y) + range) % 2 == 0) {
          adjRows = new int[] { tile.x, tile.x - 1, tile.x, tile.x + 1, tile.x, tile.x - 1, tile.x - 1 };
          adjCols = new int[] { tile.y, tile.y - 1, tile.y - 1, tile.y, tile.y + 1, tile.y + 1, tile.y };

        } else {
          adjRows = new int[] { tile.x, tile.x + 1, tile.x + 1, tile.x, tile.x - 1, tile.x, tile.x + 1 };
          adjCols = new int[] { tile.y, tile.y, tile.y + 1, tile.y + 1, tile.y, tile.y - 1, tile.y - 1 };
        }

        for (int x = 0; x < adjCols.Length; x++) {
          Vector2Int gridCell = new(adjRows[x], adjCols[x]);
          if (GameState.State.world.GetTile(gridCell.x, gridCell.y).isCovered) {
            action(gridCell);
          }
        }
      }

    }
  }
}