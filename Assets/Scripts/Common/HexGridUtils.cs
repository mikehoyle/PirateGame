using State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common {
  public class HexGridUtils {

    public static void ForEachAdjacentTileNotInVision(Vector2Int tile, Action<Vector2Int> action) {
      var visionRange = GameState.State.player.VisionRange;
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
          if (GameState.State.world.GetTile(gridCell.x, gridCell.y).IsCovered) {
            action(gridCell);
          }
        }
      }

    }
  }
}