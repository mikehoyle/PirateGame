using System;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters {
  public class TargetingHintDisplay : MonoBehaviour {
    [SerializeField] private TileBase selectedTileOverlay;
    [SerializeField] private TileBase eligibleTileOverlay;
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
      GenerateMovementTileDict();
    }
    private void GenerateMovementTileDict() {
      _movementTiles = new() {
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

    public void DisplayMovementPossibilities(UnitController unit) {
      // Put indicator under unit and show movement possibilities
      _grid.Overlay.ClearAllTiles();
      _grid.Overlay.color = Color.white;
      var gridPosition = unit.State.PositionInEncounter;
      var unitMoveRange = unit.RemainingMovement;
      _grid.Overlay.SetTile(gridPosition, selectedTileOverlay);

      for (int x = -unitMoveRange; x <= unitMoveRange; x++) {
        var yMoveRange = unitMoveRange - Math.Abs(x);
        for (int y = -yMoveRange; y <= yMoveRange; y++) {
          if (x == 0 && y == 0) {
            continue;
          }
          var tile = _grid.GetTileAtPeakElevation(
              new Vector2Int(
                  unit.State.PositionInEncounter.x + x,
                  unit.State.PositionInEncounter.y + y));
          // OPTIMIZE: memoize paths
          var path = _grid.GetPath(unit.State.PositionInEncounter, tile);
          if (unit.CouldMoveAlongPath(path)) {
            _grid.Overlay.SetTile(tile, eligibleTileOverlay);
          }
        }
      }
    }
    
    public void HandleMouseHover(Vector3 mousePosition, UnitController activeUnit) {
      var hoveredCell = _grid.TileAtScreenCoordinate(mousePosition);
      if (_lastKnownHoveredCell != hoveredCell) {
        UpdateMovementHover(hoveredCell, activeUnit);
      }
    }

    private void UpdateMovementHover(Vector3Int targetedCell, UnitController activeUnit) {
      if (activeUnit.State.PositionInEncounter == targetedCell) {
        // No need to indicate you can move where you already are
        return;
      }
      
      _lastKnownHoveredCell = targetedCell;
      ClearMovementIndicators();
      if (_grid.IsTileMovementEligible(targetedCell)) {
        var path = _grid.GetPath(activeUnit.State.PositionInEncounter, targetedCell);
        if (path != null && activeUnit.CouldMoveAlongPath(path)) {
          DisplayMovementHint(path); 
        }
      }
    }
    
    public void DisplayMovementHint(LinkedList<Vector3Int> path) {
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
      _grid.Overlay.ClearAllTiles();
    }
    
    public void DisplayAttackPossibilities(UnitController unit) {
      _grid.Overlay.ClearAllTiles();
      _grid.Overlay.color = Color.red;
      var gridPosition = unit.State.PositionInEncounter;
      _grid.Overlay.SetTile(gridPosition, selectedTileOverlay);

      for (int x = -1; x <= 1; x++) {
        var yRange = 1 - Math.Abs(x);
        for (int y = -yRange; y <= yRange; y++) {
          if (x == 0 && y == 0) {
            continue;
          }
          var tile = _grid.GetTileAtPeakElevation(
              new Vector2Int(
                  unit.State.PositionInEncounter.x + x,
                  unit.State.PositionInEncounter.y + y));
          if (_grid.IsTileMovementEligible(tile)) {
            _grid.Overlay.SetTile(tile, eligibleTileOverlay);
          }
        }
      }
    }
  }
}