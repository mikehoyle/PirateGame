using Common;
using Common.Grid;
using Controls;
using Events;
using Optional;
using RuntimeVars;
using RuntimeVars.ShipBuilder;
using State;
using StaticConfig.Builds;
using Terrain;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Construction {
  
  /// <summary>
  /// TODO(P2): Limit camera movement to avoid infinite scroll into the abyss.
  /// </summary>
  public class ShipBuilderManager : MonoBehaviour, GameControls.IShipBuilderActions {
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;
    [SerializeField] private CurrentBuildSelection currentBuildSelection;
    
    private GameControls _controls;
    private SceneTerrain _terrain;
    private PlayerState _playerState;
    private ShipSetup _shipSetup;
    private UiInteractionTracker _uiInteraction;

    private void Awake() {
      enabled = false;
      _terrain = SceneTerrain.Get();
      _shipSetup = GetComponent<ShipSetup>();
      _playerState = GameState.State.player;
      _uiInteraction = GetComponent<UiInteractionTracker>();
      Dispatch.ShipBuilder.EnterConstructionMode.RegisterListener(OnEnterConstructionMode);
      Dispatch.ShipBuilder.ExitConstructionMode.RegisterListener(OnExitConstructionMode);
      Dispatch.Common.DialogueStart.RegisterListener(OnDialogueStart);
      Dispatch.Common.DialogueEnd.RegisterListener(OnDialogueEnd);
    }

    private void OnDestroy() {
      Dispatch.ShipBuilder.EnterConstructionMode.UnregisterListener(OnEnterConstructionMode);
      Dispatch.ShipBuilder.ExitConstructionMode.UnregisterListener(OnExitConstructionMode);
      Dispatch.Common.DialogueStart.UnregisterListener(OnDialogueStart);
      Dispatch.Common.DialogueEnd.UnregisterListener(OnDialogueEnd);
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.ShipBuilder.SetCallbacks(this);
      }
      
      _controls.ShipBuilder.Enable();
      Dispatch.ShipBuilder.BuildSelected.RegisterListener(OnBuildSelected);
    }

    private void OnDisable() {
      currentBuildSelection.Clear();
      _controls.ShipBuilder.Disable();
      Dispatch.ShipBuilder.BuildSelected.UnregisterListener(OnBuildSelected);
    }

    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed || _uiInteraction.isPlayerHoveringUi) {
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
      
      if (context.canceled || _uiInteraction.isPlayerHoveringUi) {
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
          // Other builds want to go at the current elevation... which will probably also always
          // be zero. But hey, maybe we'll have stacking options in the future
          gridCell = _terrain.GetElevation((Vector2Int)gridCell);
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
        if (_playerState.ship.foundations.ContainsKey(gridCell)) {
          // This is a smelly side-effect, but it's fine for now. The goal is to not overlay
          // the indicator on the existing build.
          currentBuildSelection.tile = Option.None<Vector3Int>();
          return false;
        }
        
        // And, assert there is an adjacent foundation to attach to.
        var isValidPlacement = false;
        GridUtils.ForEachAdjacentTile(gridCell, adjacentCell => {
          if (_playerState.ship.foundations.TryGetValue(adjacentCell, out var adjacentBuild)) {
            isValidPlacement = true;
          }
        });
        
        return isValidPlacement;
      }
      
      // Build is meant to be atop a foundation, check that it is on a foundation
      if (_playerState.ship.foundations.ContainsKey(gridCell)) {
        // If movement is blocked at ground level, we assume something else is already there, so
        // we don't allow the placement.
        return !SceneTerrain.IsTileOccupied(_terrain.GetElevation((Vector2Int)gridCell));
      }
      return false;
    }

    private void OnDialogueStart() {
      _controls.TurnBasedEncounter.Disable();
    }

    private void OnDialogueEnd() {
      _controls.TurnBasedEncounter.Enable();
    }
  }
}