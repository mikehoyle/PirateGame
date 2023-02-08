using System;
using CameraControl;
using Common;
using Controls;
using Encounters;
using Encounters.Grid;
using HUD.Construction;
using HUD.MainMenu;
using State;
using StaticConfig;
using StaticConfig.Builds;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Construction {
  
  /// <summary>
  /// TODO(P2): Limit camera movement to avoid infinite scroll into the abyss.
  /// </summary>
  public class ShipBuilderManager : MonoBehaviour, GameControls.IShipBuilderActions {
    [SerializeField] private string backToMapButtonLabel = "Back to Map";
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;
    
    private GameControls _controls;
    private Vector3Int _currentHoveredTile;
    private IsometricGrid _grid;
    private BuildPlacementIndicator _placementIndicator;
    private PlayerState _playerState;
    private MainMenuController _mainMenu;
    private BuildMenuController _buildMenu;
    private ConstructableObject _selectedBuild;
    private CameraCursorMover _cameraMover;

    private void Awake() {
      _grid = IsometricGrid.Get();
      _cameraMover = GetComponent<CameraCursorMover>();
      _placementIndicator = _grid.Grid.GetComponentInChildren<BuildPlacementIndicator>();
      _playerState = GameState.State.player; 
      _buildMenu = BuildMenuController.Get();
      _buildMenu.OnBuildSelected += OnBuildSelected;
    }

    private void Start() {
      GetComponent<ShipSetup>().SetupShip();
      InitializeCamera();
      _mainMenu = MainMenuController.Get();
      _mainMenu.AddMenuItem(backToMapButtonLabel, OnBackToMap);
    }
    
    private void InitializeCamera() {
      var minX = int.MaxValue;
      var maxX = int.MinValue;
      var minY = int.MaxValue;
      var maxY = int.MinValue;
      foreach (var tileCoord in GameState.State.player.ship.components.Keys) {
        minX = Math.Min(minX, tileCoord.x);
        maxX = Math.Max(maxX, tileCoord.x);
        minY = Math.Min(minY, tileCoord.y);
        maxY = Math.Max(maxY, tileCoord.y);
      }
      
      var visualMin = _grid.Grid.CellToWorld(new Vector3Int(minX, minY, 0));
      // +1 to maxes because CellToWorld returns bottom corner of cell,
      // so top corner of cell = bottom corner of caddy-cornered cell.
      var visualMax = _grid.Grid.CellToWorld(new Vector3Int(maxX + 1, maxY + 1, 0));
      _cameraMover.Initialize(Vector3.Lerp(visualMin, visualMax, 0.5f));
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.ShipBuilder.SetCallbacks(this);
      }

      _controls.ShipBuilder.Enable();
    }

    private void OnDisable() {
      _controls.ShipBuilder.Disable();
    }

    private void OnDestroy() {
      _buildMenu.OnBuildSelected -= OnBuildSelected;
    }

    private void OnBackToMap() {
      SceneManager.LoadScene(Scenes.Name.Overworld.SceneName());
    }

    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed) {
        // Ignore start/cancel events
        return;
      }
      if (_selectedBuild == null) {
        // No build selected 
        return;
      }
      
      var mousePosition = Mouse.current.position.ReadValue();
      var gridCell = GetTargetCellForMousePosition(mousePosition);
      
      AttemptPurchase(gridCell);
    }
    
    private void AttemptPurchase(Vector3Int gridCell) {
      if (!IsValidPlacement(gridCell)) {
        return;
      }

      _playerState.inventory.DeductBuildCost(_selectedBuild);
      _playerState.ship.Add(gridCell, _selectedBuild);
      _grid.Tilemap.SetTile(gridCell, _selectedBuild.inGameTile);
      _placementIndicator.Hide();
    }

    public void OnPoint(InputAction.CallbackContext context) {
      if (!isActiveAndEnabled) {
        // Prevent attempts at handling mouse movement after scene cleanup.
        return;
      }

      if (context.canceled) {
        _placementIndicator.Hide();
        return;
      }

      if (_selectedBuild == null) {
        // Nothing selected, so nothing to indicate
        return;
      }
      
      var mousePosition = context.ReadValue<Vector2>();
      var gridCell = GetTargetCellForMousePosition(mousePosition);

      _placementIndicator.SetSprite(_selectedBuild.inGameSprite);
      if (IsValidPlacement(gridCell)) {
        _placementIndicator.ShowValidIndicator(gridCell);
      } else {
        _placementIndicator.ShowInvalidIndicator(gridCell);
      }
    }

    private Vector3Int GetTargetCellForMousePosition(Vector2 mousePosition) {
      var gridCell = _grid.TileAtScreenCoordinate(mousePosition);

      if (_selectedBuild.isFoundationTile) {
        // Always place foundations on the bottom.
        gridCell.z = 0;
      } else {
        // Other builds want to go above the current highest option.
        gridCell = _grid.GetTileAtPeakElevation((Vector2Int)gridCell);
        gridCell.z += 1;
      }

      return gridCell;
    }

    private void OnBuildSelected(object _, ConstructableObject build) {
      _selectedBuild = build;
    }
    
    private bool IsValidPlacement(Vector3Int gridCell) {
      if (!_playerState.inventory.CanAffordBuild(_selectedBuild)) {
        return false;
      }
      
      if (_selectedBuild.isFoundationTile) {
        if (gridCell.z != 0) {
          // This shouldn't be possible, but fallback logic to prevent stacking (for now)
          return false;
        }
        
        // For foundation tiles, assert the target cell is empty
        if (_playerState.ship.components.ContainsKey(gridCell)) {
          _placementIndicator.Hide();
          return false;
        }
        
        // And, assert there is an adjacent foundation to attach to.
        var isValidPlacement = false;
        IsometricGridUtils.ForEachAdjacentTile(gridCell, adjacentCell => {
          if (_playerState.ship.components.TryGetValue(adjacentCell, out var adjacentBuild)) {
            if (adjacentBuild.isFoundationTile) {
              isValidPlacement = true;
            }
          }
        });
        return isValidPlacement;
      }
      
      // Build is meant to be atop a foundation, check that a foundation is below it.
      var tileBelow = new Vector3Int(gridCell.x, gridCell.y, gridCell.z - 1);
      if (_playerState.ship.components.TryGetValue(tileBelow, out var build)) {
        if (build.isFoundationTile) {
          return true;
        }
      }
      return false;
    }
  }
}