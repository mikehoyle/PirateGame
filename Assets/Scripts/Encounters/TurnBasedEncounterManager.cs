using System;
using System.Collections.Generic;
using System.Linq;
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
    private Camera _camera;
    private IsometricGrid _grid;
    private Vector3Int _lastKnownHoveredCell = new(int.MinValue, int.MinValue, int.MinValue);

    private void Start() {
      _hud = GameObject.FindWithTag(Tags.EncounterHUD).GetComponent<EncounterHUD>();
      _camera = Camera.main;
      _grid = IsometricGrid.Get();

      _currentRound = 1;
      _unitsInEncounter = FindObjectsOfType<UnitController>().ToList();
      foreach (var unit in _unitsInEncounter) {
        _grid.Pathfinder.SetEnabled(unit.State.Position, false);
      }
      _hud.SetRound(_currentRound);
      // Set to first units turn
      // Currently turn order is completely arbitrarily based on the order we found the components
      NewUnitTurn(0);
    }
    
    private void NewUnitTurn(int unitIndex) {
      _grid.Pathfinder.SetEnabled(_unitsInEncounter[_currentUnitTurn].State.Position, false);
      _currentUnitTurn = unitIndex;
      var unit = _unitsInEncounter[_currentUnitTurn];
      _grid.Pathfinder.SetEnabled(unit.State.Position, true);
      
      // Center camera on current unit
      var camPosition = unit.WorldPosition;
      _camera.transform.position = new Vector3(camPosition.x, camPosition.y, -10);
      
      // Put indicator under unit and show movement possibilities
      _grid.Overlay.ClearAllTiles();
      var gridPosition = unit.State.Position;
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
                  _unitsInEncounter[unitIndex].State.Position.x + x,
                  _unitsInEncounter[unitIndex].State.Position.y + y));
          if (_grid.IsTileMovementEligible(tile)) {
            _grid.Overlay.SetTile(tile, eligibleTileOverlay);
          }
        }
      }
    }

    private void Update() {
      var mousePosition = Mouse.current.position;
      var hoveredCell = _grid.TileAtScreenCoordinate(mousePosition.ReadValue());
      if (_lastKnownHoveredCell != hoveredCell) {
        UpdateMovementHover(hoveredCell);
      }
    }
    private void UpdateMovementHover(Vector3Int cell) {
      if (_unitsInEncounter[_currentUnitTurn].State.Position == cell) {
        // No need to indicate you can move where you already are
        return;
      }
      
      _lastKnownHoveredCell = cell;
      _grid.TargetingDisplay.Clear();
      if (_grid.IsTileMovementEligible(cell)) {
        var path = _grid.GetPath(_unitsInEncounter[_currentUnitTurn].State.Position, cell);
        if (path != null) {
          _grid.TargetingDisplay.DisplayMovementHint(path); 
        }
      }
    }

    /// <summary>
    /// PlayerInput event
    /// </summary>
    private void OnSelect() {
      var mousePosition = Mouse.current.position;
      var gridCell = _grid.TileAtScreenCoordinate(mousePosition.ReadValue());
      var path = _grid.GetPath(_unitsInEncounter[_currentUnitTurn].State.Position, gridCell);
      
      // DO NOT SUBMIT just a test
      _unitsInEncounter[_currentUnitTurn].MoveAlongPath(path);
    }
  }
}