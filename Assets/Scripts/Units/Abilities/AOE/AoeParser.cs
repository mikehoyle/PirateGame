using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Abilities.AOE {
  /// <summary>
  /// AOE areas are defined by custom text files in which this is the key:
  /// X = The targeted tile, which is also affected
  /// O = The targeted tile, which is unaffected
  /// _ = An unaffected tile
  /// 1 = An affected tile
  /// </summary>
  public static class AoeParser {
    private const char TargetOn = 'X';
    private const char TargetOff = 'O';
    private const char Off = '_';
    private const char On = '1';

    // No big focus on efficiency here, it won't matter
    public static AreaOfEffect ParseAreaOfEffect(string raw) {
      if (string.IsNullOrEmpty(raw)) {
        return new AreaOfEffect(new List<Vector3Int>());
      }
      var lines = raw.Split("\n" , StringSplitOptions.RemoveEmptyEntries);

      var targetOffset = GetTargetOffset(lines);
      var affectedCoords = new List<Vector3Int>();
      for (int y = 0; y < lines.Length; y++) {
        for (int x = 0; x < lines[y].Length; x++) {
          if (lines[y][x] == TargetOn || lines[y][x] == On) {
            affectedCoords.Add(new Vector3Int(x, -y, 0) - targetOffset);
          }
        }
      }

      return new AreaOfEffect(affectedCoords);
    }

    private static Vector3Int GetTargetOffset(string[] lines) {
      for (int y = 0; y < lines.Length; y++) {
        for (int x = 0; x < lines[y].Length; x++) {
          if (lines[y][x] == TargetOn || lines[y][x] == TargetOff) {
            // Negate y, because our y increases top to bottom, but Y coords decrease in the
            // downward direction
            return new Vector3Int(x, -y, 0);
          } 
        }
      }
      
      Debug.LogError("Unable to find target tile in AOE.");
      return Vector3Int.zero;
    }
  }
}