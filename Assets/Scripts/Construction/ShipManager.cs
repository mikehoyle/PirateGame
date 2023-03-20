using System;
using CameraControl;
using Common;
using Common.Loading;
using Controls;
using Encounters;
using Events;
using HUD.MainMenu;
using Optional;
using RuntimeVars.Encounters;
using State;
using Terrain;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Construction {
  public class ShipManager : MonoBehaviour, GameControls.IShipManagementActions {
    [SerializeField] private string backToMapButtonLabel = "Back to Map";
    [SerializeField] private CurrentSelection currentSelection;

    private SceneTerrain _terrain;
    private ShipSetup _shipSetup;
    private MainMenuController _mainMenu;
    private GameControls _controls;
    private UiInteractionTracker _uiInteraction;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _shipSetup = GetComponent<ShipSetup>();
      _uiInteraction = GetComponent<UiInteractionTracker>();
    }

    private void Start() {
      _shipSetup.SetupShip(includeUnits: true);
      InitializeCamera();
      _mainMenu = MainMenuController.Get();
      _mainMenu.AddMenuItem(backToMapButtonLabel, OnBackToMap);
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.ShipManagement.SetCallbacks(this);
      }
      
      _controls.ShipManagement.Enable();
      Dispatch.ShipBuilder.EnterConstructionMode.RegisterListener(OnEnterConstruction);
      Dispatch.ShipBuilder.ExitConstructionMode.RegisterListener(OnExitConstruction);
    }

    private void OnDisable() {
      _controls.ShipManagement.Disable();
      Dispatch.ShipBuilder.EnterConstructionMode.UnregisterListener(OnEnterConstruction);
      Dispatch.ShipBuilder.ExitConstructionMode.UnregisterListener(OnExitConstruction);
    }

    private void OnEnterConstruction() {
      ClearSelection();
      _controls.ShipManagement.Disable();
    }

    private void OnExitConstruction() {
      _controls.ShipManagement.Enable();
    }

    private void InitializeCamera() {
      var cameraMover = GetComponent<CameraCursorMover>();
      var minX = int.MaxValue;
      var maxX = int.MinValue;
      var minY = int.MaxValue;
      var maxY = int.MinValue;
      foreach (var tileCoord in GameState.State.player.ship.foundations.Keys) {
        minX = Math.Min(minX, tileCoord.x);
        maxX = Math.Max(maxX, tileCoord.x);
        minY = Math.Min(minY, tileCoord.y);
        maxY = Math.Max(maxY, tileCoord.y);
      }
      
      var visualMin = _terrain.Grid.CellToWorld(new Vector3Int(minX, minY, 0));
      // +1 to maxes because CellToWorld returns bottom corner of cell,
      // so top corner of cell = bottom corner of caddy-cornered cell.
      var visualMax = _terrain.Grid.CellToWorld(new Vector3Int(maxX + 1, maxY + 1, 0));
      cameraMover.Initialize(Vector3.Lerp(visualMin, visualMax, 0.5f));
    }

    private void OnBackToMap() {
      SceneManager.LoadScene(Scenes.Name.Overworld.SceneName());
    }
    
    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed || _uiInteraction.isPlayerHoveringUi) {
        return;
      }

      var targetTile = _terrain.TileAtScreenCoordinate(Mouse.current.position.ReadValue());
      var clickedObject = SceneTerrain.GetTileOccupant(targetTile);
      if (currentSelection.TryGetUnit<PlayerUnitController>(out var playerUnit)
          && _terrain.IsTileEligibleForUnitOccupation(targetTile)) {
        playerUnit.SetShipPosition(targetTile);
        ClearSelection();
        return;
      }
      
      if (clickedObject != null) {
        Dispatch.Common.ObjectClicked.Raise(clickedObject.gameObject);
      }
    }

    public void OnRightClick(InputAction.CallbackContext context) {
      if (!context.performed || _uiInteraction.isPlayerHoveringUi) {
        return;
      }
      ClearSelection();
    }

    private void ClearSelection() {
      currentSelection.selectedUnit = Option.None<EncounterActor>();
      Dispatch.Encounters.UnitSelected.Raise(null);
    }

    public void OnPoint(InputAction.CallbackContext context) {
      if (!context.performed || _uiInteraction.isPlayerHoveringUi) {
        return;
      }
    }

    public void OnOpenCharacterSheet(InputAction.CallbackContext context) {
      if (!context.performed || !currentSelection.selectedUnit.TryGet(out var selectedUnit)) {
        return;
      }

      if (selectedUnit is not PlayerUnitController playerUnit) {
        return;
      }

      Dispatch.ShipBuilder.OpenCharacterSheet.Raise(playerUnit);
    }

    public void OnCloseMenu(InputAction.CallbackContext context) {
      Dispatch.ShipBuilder.CloseCharacterSheet.Raise();
      Dispatch.ShipBuilder.CloseCraftingMenu.Raise();
    }
  }
}