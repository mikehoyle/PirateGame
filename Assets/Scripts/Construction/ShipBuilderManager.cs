using System;
using CameraControl;
using Common;
using Controls;
using Encounters;
using HUD.MainMenu;
using State;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Construction {
  
  /// <summary>
  /// TODO(P0): Enable placement of more than just foundation.
  /// TODO(P1): Enable camera movement (nudge? WASD?)
  /// TODO(P2): Limit camera movement to avoid infinite scroll into the abyss.
  /// </summary>
  public class ShipBuilderManager : MonoBehaviour, GameControls.IShipBuilderActions {
    [SerializeField] private TileBase foundationTile;
    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private string overworldScene = "OverworldScene";
    [SerializeField] private string backToMapButtonLabel = "Back to Map";
    
    private GameControls _controls;
    private Vector3Int _currentHoveredTile;
    private IsometricGrid _grid;
    private CameraController _camera;
    private BuildPlacementIndicator _placementIndicator;
    private ShipState _shipState;
    private MainMenuController _mainMenu;
    private Vector3 _cameraCursor;
    private Vector3 _cameraCursorVelocity;

    private void Awake() {
      _grid = IsometricGrid.Get();
      _camera = CameraController.Get();
      _placementIndicator = _grid.Grid.GetComponentInChildren<BuildPlacementIndicator>();
      _shipState = GameState.State.Player.Ship;
    }

    private void Start() {
      InitializeCamera();
      _mainMenu = MainMenuController.Get();
      _mainMenu.AddMenuItem(backToMapButtonLabel, OnBackToMap);
    }
    
    private void InitializeCamera() {
      var minX = int.MaxValue;
      var maxX = int.MinValue;
      var minY = int.MaxValue;
      var maxY = int.MinValue;
      foreach (var tileCoord in GameState.State.Player.Ship.Foundations) {
        minX = Math.Min(minX, tileCoord.x);
        maxX = Math.Max(maxX, tileCoord.x);
        minY = Math.Min(minY, tileCoord.y);
        maxY = Math.Max(maxY, tileCoord.y);
      }
      
      var visualMin = _grid.Grid.CellToWorld(new Vector3Int(minX, minY, 0));
      // +1 to maxes because CellToWorld returns bottom corner of cell,
      // so top corner of cell = bottom corner of caddy-cornered cell.
      var visualMax = _grid.Grid.CellToWorld(new Vector3Int(maxX + 1, maxY + 1, 0));
      _cameraCursor = Vector3.Lerp(visualMin, visualMax, 0.5f);
      _camera.SnapToPoint(_cameraCursor);
    }

    private void OnEnable() {
      _controls ??= new GameControls();
      _controls.ShipBuilder.SetCallbacks(this);
      _controls.ShipBuilder.Enable();
    }

    private void OnDisable() {
      _controls.ShipBuilder.Disable();
    }

    private void Update() {
      _cameraCursor += _cameraCursorVelocity * Time.deltaTime;
      // TODO(P1): Prevent cursor from leaving the ship area.
      _camera.SetFocusPoint(_cameraCursor);
    }

    private void OnBackToMap() {
      // OPTIMIZE: Load async ideally, could lag
      SceneManager.LoadScene(overworldScene);
    }

    public void OnClick(InputAction.CallbackContext context) {
      if (context.ReadValue<float>() < 0.01) {
        // Indicates mouse-up
        return;
      }
      var mousePosition = Mouse.current.position.ReadValue();
      var gridCell = _grid.TileAtScreenCoordinate(mousePosition);

      if (!IsValidPlacement(gridCell)) {
        return;
      }
      
      GameState.State.Player.Ship.Foundations.Add(gridCell);
      _grid.Tilemap.SetTile(gridCell, foundationTile);
      _placementIndicator.Hide();
    }
    
    public void OnPoint(InputAction.CallbackContext context) {
      if (!isActiveAndEnabled) {
        // Prevent attempts at handling mouse movement after scene cleanup.
        return;
      }
      
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

    public void OnMoveCamera(InputAction.CallbackContext context) {
      _cameraCursorVelocity = context.ReadValue<Vector2>() * cameraMoveSpeed;
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