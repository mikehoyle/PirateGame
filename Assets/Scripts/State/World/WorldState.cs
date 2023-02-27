using System;
using UnityEngine;

namespace State.World {
  /// <summary>
  /// Holds World-level game state to be saved. 
  /// </summary>
  [CreateAssetMenu(menuName = "State/WorldState")]
  public class WorldState : ScriptableObject {
    [Serializable]
    public class WorldContentsDictionary : SerializableDictionary<WorldCoordinates, WorldTile> { }

    public uint currentDay;
    public WorldContentsDictionary tileContents;

    public void Awake() {
      tileContents = new WorldContentsDictionary();
    }

    public void SetTile(int x, int y, WorldTile tile) {
      var coordinates = new WorldCoordinates(x, y);
      tile.coordinates = coordinates;
      tileContents[coordinates] = tile;
    }

    public void SetTileDifficulty(int x, int y, int difficulty) {
        if (tileContents.TryGetValue(new WorldCoordinates(x, y), out var tile)) {
          tile.difficulty = difficulty;
        }
    }
    
    public WorldTile GetTile(int x, int y) {
      if (tileContents.TryGetValue(new WorldCoordinates(x, y), out var tile)) {
        return tile;
      }

      return null;
    }

    public WorldTile GetActiveTile() {
      var activeTile = GameState.State.player.overworldGridPosition;
      return GetTile(activeTile.x, activeTile.y);
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