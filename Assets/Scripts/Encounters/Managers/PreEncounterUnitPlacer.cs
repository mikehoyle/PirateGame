using Common;
using Controls;
using Encounters.Grid;
using Events;
using Optional;
using RuntimeVars;
using RuntimeVars.Encounters;
using Terrain;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Encounters.Managers {
  public class PreEncounterUnitPlacer : MonoBehaviour, GameControls.IUnitPlacementActions {
    [SerializeField] private TileCollection shipTiles;
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private string placementHintText;
    
    private GameControls _controls;
    private SceneTerrain _terrain;
    private UiInteractionTracker _uiInteraction;
    private GridIndicators _indicators;
    private Text _actionHint;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _uiInteraction = GetComponent<UiInteractionTracker>();
      _indicators = GridIndicators.Get();
      _actionHint = transform.Find("ActionHint").GetComponent<Text>();
      Dispatch.Encounters.EncounterSetUp.RegisterListener(OnEncounterSetUp);
      enabled = false;
    }

    private void OnDestroy() {
      Dispatch.Encounters.EncounterSetUp.UnregisterListener(OnEncounterSetUp);
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.UnitPlacement.SetCallbacks(this);
      }
      _controls.UnitPlacement.Enable();
      _indicators.RangeIndicator.DisplayCustomRange(shipTiles);
      _actionHint.gameObject.SetActive(true);
      _actionHint.text = placementHintText;
      Dispatch.Encounters.UnitSelected.RegisterListener(OnUnitSelected);
    }

    private void OnDisable() {
      ClearSelection();
      if (_indicators != null && _indicators.RangeIndicator != null) {
        _indicators.RangeIndicator.Clear();
      }
      _controls?.UnitPlacement.Disable();
      Dispatch.Encounters.UnitSelected.UnregisterListener(OnUnitSelected);
    }

    private void OnEncounterSetUp() {
      enabled = true;
    }

    public void OnStartEncounter(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      
      enabled = false;
      Dispatch.Encounters.EncounterStart.Raise();
    }

    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed || _uiInteraction.isPlayerHoveringUi) {
        return;
      }
      
      var targetTile = _terrain.TileAtScreenCoordinate(Mouse.current.position.ReadValue());
      var clickedObject = SceneTerrain.GetTileOccupant(targetTile);
      if (currentSelection.TryGetUnit<PlayerUnitController>(out var playerUnit)) {
        if (_terrain.IsTileEligibleForUnitOccupation(targetTile) && shipTiles.Contains(targetTile)) {
          playerUnit.SetPosition(targetTile);
          ClearSelection();
          return;
        }
        if (playerUnit.Position == targetTile) {
          ClearSelection();
          return;
        }
      }
      
      if (clickedObject != null) {
        Dispatch.Common.ObjectClicked.Raise(clickedObject.gameObject);
      }
    }

    private void ClearSelection() {
      currentSelection.SelectUnit(null);
    }

    private void OnUnitSelected(EncounterActor unit) {
      if (unit == null || unit is PlayerUnitController) {
        if (_indicators.RangeIndicator != null) {
          _indicators.RangeIndicator.DisplayCustomRange(shipTiles);
        }
      }
    }
  }
}