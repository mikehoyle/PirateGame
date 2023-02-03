using System;
using System.Collections.Generic;
using System.Linq;
using CameraControl;
using Controls;
using HUD.Encounter;
using Pathfinding;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Encounters {
  public class TurnBasedEncounterManager : MonoBehaviour, GameControls.ITurnBasedEncounterActions {
    private int _currentRound;
    private List<UnitController> _unitsInEncounter = new();
    private int _currentUnitTurn = 0;
    private EncounterHUD _hud;
    private CameraController _camera;
    private IsometricGrid _grid;
    private TargetingHintDisplay _targetingDisplay;
    private GameControls _controls;
    private ActionMenuController _actionMenu;
    private UnitAction _currentlySelectedAction;
    private EncounterPathfindingGrid _terrain;

    private UnitController ActiveUnit => _unitsInEncounter[_currentUnitTurn];

    private void Awake() {
      _hud = GameObject.FindWithTag(Tags.EncounterHUD).GetComponent<EncounterHUD>();
      _actionMenu = ActionMenuController.Get();
      _camera = Camera.main.GetComponent<CameraController>();
      _grid = IsometricGrid.Get();
      _terrain = EncounterPathfindingGrid.Get();
      _targetingDisplay = _grid.Grid.GetComponentInChildren<TargetingHintDisplay>();

      _currentRound = 1;
    }

    private void Start() {
      _unitsInEncounter = FindObjectsOfType<UnitController>().ToList();
      foreach (var unit in _unitsInEncounter) {
        _terrain.SetEnabled(unit.EncounterMetadata.Position, false);
      }
      
      _hud.SetRound(_currentRound);
      // Set to first units turn
      // Currently turn order is completely arbitrarily based on the order we found the components
      OnNewUnitTurn(0);
    }
    
    private void OnEnable() {
      if (_controls == null) {
        _controls ??= new GameControls();
        _controls.TurnBasedEncounter.SetCallbacks(this);
      }

      _controls.TurnBasedEncounter.Enable();
    }

    private void OnDisable() {
      _controls.TurnBasedEncounter.Disable();
    }
    
    private void OnNewUnitTurn(int unitIndex) {
      // TODO(P1): Overhaul the way dynamic obstacles such as units are handled in encounters.
      //     These scattered interactions with the pathfinder are very messy and error-prone.
      _terrain.SetEnabled(ActiveUnit.EncounterMetadata.Position, false);
      _currentUnitTurn = unitIndex;
      _terrain.SetEnabled(ActiveUnit.EncounterMetadata.Position, true);
      ActiveUnit.EncounterMetadata.StartTurn();
      
      // Center camera on current unit
      // TODO(P1): Camera controls in encounter
      _camera.SetFocusPoint(ActiveUnit.WorldPosition);
      
      // Display action options, and default select the first
      _actionMenu.DisplayMenuItemsForUnit(ActiveUnit);
      SelectAction(ActiveUnit.EncounterMetadata.AvailableActions[0]);
    }

    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      
      var gridCell = _grid.TileAtScreenCoordinate(Mouse.current.position.ReadValue());
      switch (_currentlySelectedAction) {
        case UnitAction.Move:
          AttemptMove(gridCell);
          return;
        case UnitAction.AttackMelee:
          AttemptAttack(gridCell);
          return;
      }
    }
    private void AttemptAttack(Vector3Int gridCell) {
      foreach (var unit in _unitsInEncounter) {
        if (unit.EncounterMetadata.Position == gridCell && ActiveUnit.IsUnitEnemy(unit)) {
          // TODO(P0): Make attacking not completely random;
          unit.EncounterMetadata.CurrentHp -= Random.Range(3, 6);
          unit.EncounterMetadata.AvailableActions.Remove(UnitAction.AttackMelee);
          _actionMenu.DisplayMenuItemsForUnit(unit);
          SelectAction(UnitAction.None);
          return;
        }
      }
    }

    private void AttemptMove(Vector3Int gridCell) {
      var path = _terrain.GetPath(ActiveUnit.EncounterMetadata.Position, gridCell);

      var formerPosition = ActiveUnit.EncounterMetadata.Position;
      if (path != null && ActiveUnit.MoveAlongPath(path, OnMoveComplete)) {
        _terrain.SetEnabled(formerPosition, true);
        _targetingDisplay.ClearAll();
        _controls.TurnBasedEncounter.Disable();
      }
    }

    private void OnMoveComplete() {
      _camera.SetFocusPoint(ActiveUnit.WorldPosition);
      if (ActiveUnit.EncounterMetadata.RemainingMovement <= 0) {
        ActiveUnit.EncounterMetadata.AvailableActions.Remove(UnitAction.Move);
        _actionMenu.DisplayMenuItemsForUnit(ActiveUnit);
        _currentlySelectedAction = UnitAction.None;
      }
      SelectAction(_currentlySelectedAction);
      _controls.TurnBasedEncounter.Enable();
    }

    public void OnPoint(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }

      // TODO(P1): All these switch-cases are ugly and not maintainable. Make a better way. 
      switch (_currentlySelectedAction) {
        case UnitAction.Move:
          _targetingDisplay.HandleMouseHover(context.ReadValue<Vector2>(), ActiveUnit);
          return;
      }
    }
    
    public void OnSelectActionOne(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      SelectActionIndex(1);
    }
    public void OnSelectActionTwo(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      SelectActionIndex(2);
    }
    public void OnSelectActionThree(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      SelectActionIndex(3);
    }
    public void OnSelectActionFour(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      SelectActionIndex(4);
    }
    public void OnSelectActionFive(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      SelectActionIndex(5);
    }

    private void SelectActionIndex(int index) {
      if (ActiveUnit.EncounterMetadata.CapableActions.Count < index) {
        return;
      }
      var action = ActiveUnit.EncounterMetadata.CapableActions[index - 1];
      if (!ActiveUnit.EncounterMetadata.AvailableActions.Contains(action)) {
        return;
      }
      SelectAction(action);
    }

    private void SelectAction(UnitAction unitAction) {
      _currentlySelectedAction = unitAction;
      _targetingDisplay.ClearAll();

      switch (unitAction) {
        case UnitAction.Move:
          _targetingDisplay.DisplayMovementPossibilities(ActiveUnit);
          return;
        case UnitAction.AttackMelee:
          _targetingDisplay.DisplayAttackPossibilities(ActiveUnit);
          return;
        case UnitAction.EndTurn:
          EndTurn();
          return;
      }
    }
    
    private void EndTurn() {
      var nextTurn = _currentUnitTurn + 1;
      if (nextTurn >= _unitsInEncounter.Count) {
        _currentRound++;
        _hud.SetRound(_currentRound);
        nextTurn = 0;
      }
      OnNewUnitTurn(nextTurn);
    }
  }
}