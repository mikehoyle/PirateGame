using System;
using System.Collections.Generic;
using Common.Events;
using Pathfinding;
using RuntimeVars.Encounters;
using StaticConfig.Units;
using Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters {
  public class MovementPathIndicator : MonoBehaviour {
    [SerializeField] private Vector3Event mouseHoverEvent;
    [SerializeField] private UnitAbility movementAbility;
    [SerializeField] private CurrentSelection currentSelection;
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
    private IsometricGrid _grid;
    private Vector3Int _lastKnownHoveredCell = new(int.MinValue, int.MinValue, int.MinValue);
    private EncounterTerrain _terrain;

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
      _grid = IsometricGrid.Get();
      _terrain = EncounterTerrain.Get();
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
      mouseHoverEvent.RegisterListener(OnMouseHover);
    }

    private void OnDisable() {
      mouseHoverEvent.UnregisterListener(OnMouseHover);
    }

    private void OnMouseHover(Vector3 mousePosition) {
      if (currentSelection.selectedAbility != movementAbility || currentSelection.selectedUnit == null) {
        return;
      }
      
      var hoveredCell = _grid.TileAtScreenCoordinate(mousePosition);
      if (_lastKnownHoveredCell != hoveredCell) {
        var unitPosition = currentSelection.selectedUnit.State.encounterState.position;
        var movementRange = currentSelection.selectedUnit.State.encounterState.remainingMovement;
        UpdateMovementHover(hoveredCell, unitPosition, movementRange);
      }
    }

    private void UpdateMovementHover(Vector3Int targetedCell, Vector3Int unitPosition, int movementRange) {
      if (unitPosition == targetedCell) {
        // No need to indicate you can move where you already are
        return;
      }
      
      _lastKnownHoveredCell = targetedCell;
      ClearMovementIndicators();
      if (_grid.IsTileMovementEligible(targetedCell)) {
        var path = _terrain.GetPath(unitPosition, targetedCell);
        if (path != null && UnitController.IsPathViable(path) && path.Count - 1 <= movementRange) {
          DisplayMovementHint(path); 
        }
      }
    }
    
    private void DisplayMovementHint(LinkedList<Vector3Int> path) {
      _tilemap.ClearAllTiles();
      DisplayMovementHintInternal(path.First);
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

    private void ClearMovementIndicators() {
      _tilemap.ClearAllTiles();
    }

    public void ClearAll() {
      ClearMovementIndicators();
    }
  }
}