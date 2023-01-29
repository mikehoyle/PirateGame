using System;
using CameraControl;
using Common;
using Controls;
using Encounters;
using State;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Construction {
  
  /// <summary>
  /// TODO(P0): Enable placement of more than just foundation.
  /// TODO(P1): Enable camera movement (nudge? WASD?)
  /// TODO(P2): Limit camera movement to avoid infinite scroll into the abyss.
  /// </summary>
  public class ShipBuilderManager : MonoBehaviour, GameControls.IShipBuilderActions {
    [SerializeField] private TileBase foundationTile;
    
    private GameControls _controls;
    private Vector3Int _currentHoveredTile;
    private IsometricGrid _grid;
    private CameraController _camera;
    private BuildPlacementIndicator _placementIndicator;
    private ShipState _shipState;

    private void Awake() {
      _grid = IsometricGrid.Get();
      _camera = Camera.main.GetComponent<CameraController>();
      _placementIndicator = _grid.Grid.GetComponentInChildren<BuildPlacementIndicator>();
      _shipState = GameState.State.Player.ShipState;
      _controls = new GameControls();
      _controls.ShipBuilder.SetCallbacks(this);
      _controls.ShipBuilder.Enable();
    }

    private void Start() {
      Debug.Log($"Ship state foundations:");
      foreach (var item in _shipState.Foundations) {
        Debug.Log($"{item}");
      }
    }

    public void OnClick(InputAction.CallbackContext context) {
      if (context.ReadValue<float>() < 0.01) {
        // Indicates mouse-up
        return;
      }
      var mousePosition = Mouse.current.position.ReadValue();
      var gridCell = _grid.TileAtScreenCoordinate(mousePosition);
      
      Debug.Log($"Clicked cell: {gridCell}");

      if (!IsValidPlacement(gridCell)) {
        return;
      }
      
      GameState.State.Player.ShipState.Foundations.Add(gridCell);
      _grid.Tilemap.SetTile(gridCell, foundationTile);
      _placementIndicator.Hide();
    }
    
    public void OnPoint(InputAction.CallbackContext context) {
      var mousePosition = context.ReadValue<Vector2>();
      var gridCell = _grid.TileAtScreenCoordinate(mousePosition);
      
      if (_shipState.Foundations.Contains(gridCell)) {
        _placementIndicator.Hide();
        return;
      }

      if (IsValidPlacement(gridCell)) {
        _placementIndicator.ShowValidIndicator(gridCell);
      } else {
        _placementIndicator.ShowInvalidIndicator(gridCell);
      }
    }
    
    private bool IsValidPlacement(Vector3Int gridCell) {
      var isValidPlacement = false;
      IsometricGridUtils.ForEachAdjacentTile(gridCell, adjacentCell => {
        if (_shipState.Foundations.Contains(adjacentCell)) {
          isValidPlacement = true;
        }
      });
      return isValidPlacement;
    }
  }
}