using System;
using System.Collections.Generic;
using Common.Events;
using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class PathIndicator : MonoBehaviour {
    [SerializeField] private EmptyGameEvent playerTurnEndEvent;
    [SerializeField] private List<TileBase> arrowIndicatorTiles;
    
    private Tilemap _tilemap;
    
    /// <summary>
    /// Map movement tiles based on some kind-of hacky bit work.
    /// +-(is_origin)
    /// | +-(is_destination)
    /// | | +-(+x)
    /// | | | +-(-x)
    /// | | | | +-(+y)
    /// | | | | | +-(-y)
    /// 0 0 0 0 0 0
    /// </summary>
    private Dictionary<Flags, TileBase> _movementTiles;
    private Vector3Int _lastKnownHoveredCell = new(int.MinValue, int.MinValue, int.MinValue);
    private SceneTerrain _terrain;

    [Flags]
    private enum Flags {
      None = 0,
      Down = 0b000001,
      Up = 0b000010,
      Left = 0b000100,
      Right = 0b001000,
      IsDestination = 0b010000,
      IsOrigin = 0b100000,
    }

    private void Awake() {
      _tilemap = GetComponent<Tilemap>();
      _terrain = SceneTerrain.Get();
      GenerateMovementTileDict();
    }
    private void GenerateMovementTileDict() {
      _movementTiles ??= new() {
          [Flags.Down | Flags.Up] = arrowIndicatorTiles[0],
          [Flags.Left | Flags.Right] = arrowIndicatorTiles[1],
          [Flags.Down | Flags.Right] = arrowIndicatorTiles[2],
          [Flags.Right | Flags.Up] = arrowIndicatorTiles[3],
          [Flags.Up | Flags.Left] = arrowIndicatorTiles[4],
          [Flags.Left | Flags.Down] = arrowIndicatorTiles[5],
          [Flags.Down | Flags.IsDestination] = arrowIndicatorTiles[6],
          [Flags.Right | Flags.IsDestination] = arrowIndicatorTiles[7],
          [Flags.Up | Flags.IsDestination] = arrowIndicatorTiles[8],
          [Flags.Left | Flags.IsDestination] = arrowIndicatorTiles[9],
          [Flags.Down | Flags.IsOrigin] = arrowIndicatorTiles[10],
          [Flags.Right | Flags.IsOrigin] = arrowIndicatorTiles[11],
          [Flags.Up | Flags.IsOrigin] = arrowIndicatorTiles[12],
          [Flags.Left | Flags.IsOrigin] = arrowIndicatorTiles[13],
      };
    }

    private void OnEnable() {
      playerTurnEndEvent.RegisterListener(OnPlayerTurnEnd);
    }

    private void OnDisable() {
      playerTurnEndEvent.RegisterListener(OnPlayerTurnEnd);
    }

    private void OnPlayerTurnEnd() {
      enabled = false;
    }

    public void DisplayMovementPath(Vector3Int unitPosition, int movementRange, Vector3Int hoveredTile) {
      enabled = true;
      
      if (_lastKnownHoveredCell != hoveredTile) {
        UpdateMovementHover(hoveredTile, unitPosition, movementRange);
      }
    }

    private void UpdateMovementHover(Vector3Int targetedCell, Vector3Int unitPosition, int movementRange) {
      if (unitPosition == targetedCell) {
        // No need to indicate you can move where you already are
        return;
      }
      
      _lastKnownHoveredCell = targetedCell;
      Clear();
      var path = _terrain.GetPath(unitPosition, targetedCell);
      if (path.IsViableAndWithinRange(movementRange)) {
        Clear();
        DisplayMovementHintInternal(path.Path.First); 
      }
    }

    private void DisplayMovementHintInternal(LinkedListNode<Vector3Int> node) {
      if (node == null) {
        return;
      }

      var flags = Flags.None;
      if (node.Previous == null) {
        flags |= Flags.IsOrigin;
      } else {
        flags |= GetPathDiff(node.Previous.Value, node.Value);
      }
      
      if (node.Next == null) {
        flags |= Flags.IsDestination;
      } else {
        flags |= GetPathDiff(node.Next.Value, node.Value);
      }

      // TODO(P3): All this is very error-prone, because not all flags are
      //     covered by the dict. A change in pathfinding (for example) could break it.
      _tilemap.SetTile(node.Value, _movementTiles[flags]);
      DisplayMovementHintInternal(node.Next);
    }

    private Flags GetPathDiff(Vector3Int origin, Vector3Int destination) {
      var result = Flags.None;
      var diff = destination - origin;
      if (diff.y < 0) {
        result |= Flags.Up;
      }
      if (diff.y > 0) {
        result |= Flags.Down;
      }
      if (diff.x > 0) {
        result |= Flags.Left;
      }
      if (diff.x < 0) {
        result |= Flags.Right;
      }
      
      return result;
    }

    public void Clear() {
      _tilemap.ClearAllTiles();
    }
  }
}