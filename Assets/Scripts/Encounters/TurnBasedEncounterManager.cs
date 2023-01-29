using System;
using System.Collections.Generic;
using System.Linq;
using CameraControl;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Encounters {
  public class TurnBasedEncounterManager : MonoBehaviour {
    [SerializeField] private TileBase selectedTileOverlay;
    [SerializeField] private TileBase eligibleTileOverlay;
    
    private int _currentRound;
    private List<UnitController> _unitsInEncounter = new();
    private int _currentUnitTurn = 0;
    private EncounterHUD _hud;
    private CameraController _camera;
    private IsometricGrid _grid;
    private Vector3Int _lastKnownHoveredCell = new(int.MinValue, int.MinValue, int.MinValue);
    private bool _userInteractionBlocked = false;
    private TargetingHintDisplay _targetingDisplay;

    private void Start() {
      _hud = GameObject.FindWithTag(Tags.EncounterHUD).GetComponent<EncounterHUD>();
      _camera = Camera.main.GetComponent<CameraController>();
      _grid = IsometricGrid.Get();
      _targetingDisplay = _grid.Grid.transform.Find("TargetingHint").GetComponent<TargetingHintDisplay>();

      _currentRound = 1;
      _unitsInEncounter = FindObjectsOfType<UnitController>().ToList();
      foreach (var unit in _unitsInEncounter) {
        _grid.Pathfinder.SetEnabled(unit.State.PositionInEncounter, false);
      }
      _hud.SetRound(_currentRound);
      // Set to first units turn
      // Currently turn order is completely arbitrarily based on the order we found the components
      NewUnitTurn(0);
    }
    
    private void NewUnitTurn(int unitIndex) {
      _grid.Pathfinder.SetEnabled(_unitsInEncounter[_currentUnitTurn].State.PositionInEncounter, false);
      _currentUnitTurn = unitIndex;
      var unit = _unitsInEncounter[_currentUnitTurn];
      _grid.Pathfinder.SetEnabled(unit.State.PositionInEncounter, true);
      
      // Center camera on current unit
      _camera.SetFocusPoint(unit.WorldPosition);
      
      // Put indicator under unit and show movement possibilities
      _grid.Overlay.ClearAllTiles();
      var gridPosition = unit.State.PositionInEncounter;
      var unitMoveRange = unit.State.MovementRange;
      _grid.Overlay.SetTile(gridPosition, selectedTileOverlay);

      for (int x = -unitMoveRange; x <= unitMoveRange; x++) {
        var yMoveRange = unitMoveRange - Math.Abs(x);
        for (int y = -yMoveRange; y <= yMoveRange; y++) {
          if (x == 0 && y == 0) {
            continue;
          }
          var tile = _grid.GetTileAtPeakElevation(
              new Vector2Int(
                  _unitsInEncounter[unitIndex].State.PositionInEncounter.x + x,
                  _unitsInEncounter[unitIndex].State.PositionInEncounter.y + y));
          // OPTIMIZE: memoize paths
          var path = _grid.GetPath(_unitsInEncounter[_currentUnitTurn].State.PositionInEncounter, tile);
          if (_unitsInEncounter[unitIndex].CouldMoveAlongPath(path)) {
            _grid.Overlay.SetTile(tile, eligibleTileOverlay);
          }
        }
      }
    }

    private void Update() {
      HandleMouseHover();
    }
    
    private void HandleMouseHover() {
      if (_userInteractionBlocked) {
        return;
      }
      
      var mousePosition = Mouse.current.position;
      var hoveredCell = _grid.TileAtScreenCoordinate(mousePosition.ReadValue());
      if (_lastKnownHoveredCell != hoveredCell) {
        UpdateMovementHover(hoveredCell);
      }
    }

    private void UpdateMovementHover(Vector3Int cell) {
      if (_unitsInEncounter[_currentUnitTurn].State.PositionInEncounter == cell) {
        // No need to indicate you can move where you already are
        return;
      }
      
      _lastKnownHoveredCell = cell;
      _targetingDisplay.Clear();
      if (_grid.IsTileMovementEligible(cell)) {
        var path = _grid.GetPath(_unitsInEncounter[_currentUnitTurn].State.PositionInEncounter, cell);
        if (path != null && _unitsInEncounter[_currentUnitTurn].CouldMoveAlongPath(path)) {
          _targetingDisplay.DisplayMovementHint(path); 
        }
      }
    }

    /// <summary>
    /// PlayerInput event
    /// </summary>
    private void OnSelect() {
      // TODO(P2): Handle this interaction blocking more cleanly by making better use of InputAction
      //     generated code, and simply deactivating the control scheme.
      if (_userInteractionBlocked) {
        return;
      }
      
      var mousePosition = Mouse.current.position;
      var gridCell = _grid.TileAtScreenCoordinate(mousePosition.ReadValue());
      var path = _grid.GetPath(_unitsInEncounter[_currentUnitTurn].State.PositionInEncounter, gridCell);
      
      
      if (path != null && _unitsInEncounter[_currentUnitTurn].MoveAlongPath(path, OnUnitActionComplete)) {
        _userInteractionBlocked = true;
      }
    }

    private void OnUnitActionComplete() {
      _userInteractionBlocked = false;
      // TODO(P0): currently swap turn after simple movement, obviously allow more than just movement
      NewUnitTurn((_currentUnitTurn + 1) % _unitsInEncounter.Count);
    }
  }
}