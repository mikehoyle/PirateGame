using System;
using Common.Events;
using Controls;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Encounters {
  public class TurnBasedEncounterManager : MonoBehaviour, GameControls.ITurnBasedEncounterActions {
    [SerializeField] private UnitSelectedEvent unitSelectedEvent;
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private Vector3Event mouseHoverEvent;
    [SerializeField] private EmptyGameEvent endPlayerTurnEvent;
    [SerializeField] private EmptyGameEvent endEnemyTurnEvent;
    
    private GameControls _controls;
    private IsometricGrid _grid;

    private void Awake() {
      _grid = IsometricGrid.Get();
      currentSelection.Reset();
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.TurnBasedEncounter.SetCallbacks(this);
      }
      _controls.TurnBasedEncounter.Enable();
      endEnemyTurnEvent.RegisterListener(OnStartPlayerTurn);
    }

    private void OnDisable() {
      _controls.TurnBasedEncounter.Disable();
      endEnemyTurnEvent.UnregisterListener(OnStartPlayerTurn);
    }

    private void OnStartPlayerTurn() {
      _controls.TurnBasedEncounter.Enable();
    }
    
    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }

      var mousePosition = Mouse.current.position.ReadValue();
      var ray = Camera.main.ScreenPointToRay(mousePosition);
      var hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
      if (hit.collider != null && hit.collider.TryGetComponent<UnitController>(out var clickedUnit)) {
        currentSelection.selectedUnit = clickedUnit;
        // TODO(P0): this is a hack, fix it to actually have ability selection.
        currentSelection.selectedAbility = clickedUnit.GetAllCapableAbilities()[0];
        unitSelectedEvent.Raise(clickedUnit);
        return;
      }

      var targetTile = _grid.TileAtScreenCoordinate(mousePosition);
      if (currentSelection.selectedAbility != null && currentSelection.selectedUnit != null) {
        currentSelection.selectedAbility.TryExecute(currentSelection.selectedUnit, targetTile);
      }
    }
    
    public void OnPoint(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }

      mouseHoverEvent.Raise(context.ReadValue<Vector2>());
    }

    public void OnSelectActionOne(InputAction.CallbackContext context) {
      // TODO
    }
    public void OnSelectActionTwo(InputAction.CallbackContext context) {
      // TODO
    }
    public void OnSelectActionThree(InputAction.CallbackContext context) {
      // TODO
    }
    public void OnSelectActionFour(InputAction.CallbackContext context) {
      // TODO
    }
    public void OnSelectActionFive(InputAction.CallbackContext context) {
      // TODO
    }

    public void OnEndTurn(InputAction.CallbackContext context) {
      _controls.TurnBasedEncounter.Disable();
      endPlayerTurnEvent.Raise();
    }
  }
}