using System;
using CameraControl;
using Common;
using Controls;
using HUD.MainMenu;
using RuntimeVars.ShipBuilder.Events;
using State;
using Terrain;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Construction {
  public class ShipManager : MonoBehaviour, GameControls.IShipManagementActions {
    [SerializeField] private string backToMapButtonLabel = "Back to Map";
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;

    private SceneTerrain _terrain;
    private ShipSetup _shipSetup;
    private MainMenuController _mainMenu;
    private GameControls _controls;
    private UiInteractionTracker _uiInteraction;
    private CameraController _cameraController;
    private LayerMask _unitInteractionLayer;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _shipSetup = GetComponent<ShipSetup>();
      _uiInteraction = GetComponent<UiInteractionTracker>();
      _cameraController = CameraController.Get();
    }

    private void Start() {
      _unitInteractionLayer = LayerMask.GetMask("Clickable");
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
      shipBuilderEvents.enterConstructionMode.RegisterListener(OnEnterConstruction);
      shipBuilderEvents.exitConstructionMode.RegisterListener(OnExitConstruction);
    }

    private void OnDisable() {
      _controls.ShipManagement.Disable();
      shipBuilderEvents.enterConstructionMode.UnregisterListener(OnEnterConstruction);
      shipBuilderEvents.exitConstructionMode.UnregisterListener(OnExitConstruction);
    }

    private void OnEnterConstruction() {
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
      
      
      var clickedObject = _cameraController.RaycastFromMousePosition(_unitInteractionLayer).collider?.gameObject;
      var targetTile = _terrain.TileAtScreenCoordinate(Mouse.current.position.ReadValue());
      
      if (clickedObject != null) {
        shipBuilderEvents.objectClicked.Raise(clickedObject.gameObject);
      }
    }
    
    public void OnPoint(InputAction.CallbackContext context) {
      if (!context.performed || _uiInteraction.isPlayerHoveringUi) {
        return;
      }
      
      
    }
  }
}