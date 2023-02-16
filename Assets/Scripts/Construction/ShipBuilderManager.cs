using Common;
using Controls;
using Optional;
using RuntimeVars.ShipBuilder;
using RuntimeVars.ShipBuilder.Events;
using State;
using StaticConfig.Builds;
using Terrain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Construction {
  
  /// <summary>
  /// TODO(P2): Limit camera movement to avoid infinite scroll into the abyss.
  /// </summary>
  public class ShipBuilderManager : MonoBehaviour, GameControls.IShipBuilderActions {
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;
    [SerializeField] private CurrentBuildSelection currentBuildSelection;
    
    private GameControls _controls;
    private SceneTerrain _terrain;
    private PlayerState _playerState;
    private ShipSetup _shipSetup;

    private void Awake() {
      enabled = false;
      _terrain = SceneTerrain.Get();
      _shipSetup = GetComponent<ShipSetup>();
      _playerState = GameState.State.player;
      shipBuilderEvents.enterConstructionMode.RegisterListener(OnEnterConstructionMode);
      shipBuilderEvents.exitConstructionMode.RegisterListener(OnExitConstructionMode);
    }

    private void OnDestroy() {
      shipBuilderEvents.enterConstructionMode.UnregisterListener(OnEnterConstructionMode);
      shipBuilderEvents.exitConstructionMode.UnregisterListener(OnExitConstructionMode);
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.ShipBuilder.SetCallbacks(this);
      }
      
      _controls.ShipBuilder.Enable();
      shipBuilderEvents.buildSelected.RegisterListener(OnBuildSelected);
    }

    private void OnDisable() {
      currentBuildSelection.Clear();
      _controls.ShipBuilder.Disable();
      shipBuilderEvents.buildSelected.UnregisterListener(OnBuildSelected);
    }

    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed || EventSystem.current.IsPointerOverGameObject()) {
        // Ignore start/cancel events and UI-bound events
        return;
      }
      
      AttemptPurchase();
    }

    private void OnEnterConstructionMode() {
      enabled = true;
    }

    private void OnExitConstructionMode() {
      enabled = false;
    }
    
    
    private void AttemptPurchase() {
      if (!currentBuildSelection.build.TryGet(out var build) || !currentBuildSelection.tile.TryGet(out var tile)) {
        return;
      }
      
      if (!IsValidPlacement(tile)) {
        return;
      }

      _playerState.inventory.DeductBuildCost(build);
      _playerState.ship.Add(tile, build);
      _shipSetup.AddBuild(tile, build);
      currentBuildSelection.tile = Option.None<Vector3Int>();
    }

    public void OnPoint(InputAction.CallbackContext context) {
      if (!isActiveAndEnabled) {
        // Prevent attempts at handling mouse movement after scene cleanup.
        return;
      }
      
      if (context.canceled || EventSystem.current.IsPointerOverGameObject()) {
        currentBuildSelection.tile = Option.None<Vector3Int>();
        return;
      }
      
      var tile = GetTargetCellForMousePosition(context.ReadValue<Vector2>());
      currentBuildSelection.tile = Option.Some(tile);
      currentBuildSelection.isValidPlacement = IsValidPlacement(tile);
    }

    private Vector3Int GetTargetCellForMousePosition(Vector2 mousePosition) {
      var gridCell = _terrain.TileAtScreenCoordinate(mousePosition);

      currentBuildSelection.build.MatchSome(build => {
        if (build.isFoundationTile) {
          // Always place foundations on the bottom.
          gridCell.z = 0;
        } else {
          // Other builds want to go above the current highest option.
          gridCell = _terrain.GetElevation((Vector2Int)gridCell);
          gridCell.z += 1;
        }
      });
      
      

      return gridCell;
    }

    private void OnBuildSelected(ConstructableObject build) {
      currentBuildSelection.build = Option.Some(build);
    }
    
    private bool IsValidPlacement(Vector3Int gridCell) {
      if (!currentBuildSelection.build.TryGet(out var selectedBuild)) {
        return false;
      }
      
      if (!_playerState.inventory.CanAffordBuild(selectedBuild)) {
        return false;
      }
      
      if (selectedBuild.isFoundationTile) {
        if (gridCell.z != 0) {
          // This shouldn't be possible, but fallback logic to prevent stacking (for now)
          return false;
        }
        
        // For foundation tiles, assert the target cell is empty
        if (_playerState.ship.components.ContainsKey(gridCell)) {
          // This is a smelly side-effect, but it's fine for now. 
          currentBuildSelection.tile = Option.None<Vector3Int>();
          return false;
        }
        
        // And, assert there is an adjacent foundation to attach to.
        var isValidPlacement = false;
        GridUtils.ForEachAdjacentTile(gridCell, adjacentCell => {
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