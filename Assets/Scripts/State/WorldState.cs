using System;
using System.Collections.Generic;
using UnityEngine;

namespace State {
  /// <summary>
  /// Holds World-level game state to be saved. 
  /// </summary>
  [Serializable]
  public class WorldState {
    public uint CurrentDay = 1;
    public Dictionary<Vector2Int, WorldTileContents> TileContents = new();
  }
}