using System;
using UnityEngine;
using Zen.Hexagons;

namespace State.World {
  /// <summary>
  /// Holds World-level game state to be saved. 
  /// </summary>
  [CreateAssetMenu(menuName = "State/WorldState")]
  public class WorldState : ScriptableObject {
    [Serializable]
    public class WorldContentsDictionary : SerializableDictionary<HexOffsetCoordinates, WorldTile> { }
    public WorldContentsDictionary tileContents;

    private WorldState() {
      tileContents = new();
    }

    public void UpdateTile(WorldTile tile) {
      tileContents[tile.coordinates] = tile;
    }

    // Does not overwrite
    public void TrySetTile(WorldTile tile) {
      if (!tileContents.ContainsKey(tile.coordinates)) {
        UpdateTile(tile);
      }
    }
    
    public WorldTile GetTile(int x, int y) {
      return GetTile(HexOffsetCoordinates.From(x, y));
    }
    
    public WorldTile GetTile(HexOffsetCoordinates coordinates) {
      if (tileContents.TryGetValue(coordinates, out var tile)) {
        return tile;
      }

      return null;
    }

    public WorldTile GetActiveTile() {
      var activeTile = GameState.State.player.overworldGridPosition;
      return GetTile(activeTile.x, activeTile.y);
    }
  }
}