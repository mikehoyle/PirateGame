using System;
using System.Collections.Generic;
using Common.Grid;
using Overworld.MapGeneration;
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
    public HashSet<HexPath> outpostBorders;

    private WorldState() {
      tileContents = new();
      outpostBorders = new();
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

    public bool CanExecuteMove(Vector3Int to) {
      var destination = GetTile(to.x, to.y);
      if (!destination.isTraversable) {
        return false;
      }

      foreach (var border in outpostBorders) {
        foreach (var edge in border.edges) {
          if (edge.CrossesBorder(
              HexOffsetCoordinates.From((Vector3Int)GameState.State.player.overworldGridPosition),
              HexOffsetCoordinates.From(to))) {
            return false;
          }
        }
      }
      return true;
    }

    public WorldTile GetActiveTile() {
      var activeTile = GameState.State.player.overworldGridPosition;
      return GetTile(activeTile.x, activeTile.y);
    }
  }
}