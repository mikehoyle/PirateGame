using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace State.World {
  /// <summary>
  /// Holds World-level game state to be saved. 
  /// </summary>
  [Serializable]
  public class WorldState {
    public uint CurrentDay = 1;
    public Dictionary<WorldCoordinates, WorldTile> TileContents = new();

    public void SetTile(int x, int y, WorldTile tile) {
      var coordinates = new WorldCoordinates(x, y);
      tile.Coordinates = coordinates;
      TileContents[coordinates] = tile;
    }
    
    public WorldTile GetTile(int x, int y) {
      if (TileContents.TryGetValue(new WorldCoordinates(x, y), out var tile)) {
        return tile;
      }

      return null;
    }
  }


  [Serializable]
  public struct WorldCoordinates {
    public int X;
    public int Y;

    public WorldCoordinates(int x, int y) {
      X = x;
      Y = y;
    }
  }
}