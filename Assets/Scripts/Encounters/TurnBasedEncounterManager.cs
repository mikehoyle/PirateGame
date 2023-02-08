﻿using System;
using CameraControl;
using Common.Events;
using Controls;
using Encounters.Grid;
using Optional.Unsafe;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using StaticConfig.Units;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Encounters {
  public class TurnBasedEncounterManager : MonoBehaviour, GameControls.ITurnBasedEncounterActions {
    [SerializeField] private ObjectClickedEvent objectClickedEvent;
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private Vector3Event mouseHoverEvent;
    [SerializeField] private EmptyGameEvent endPlayerTurnEvent;
    [SerializeField] private EmptyGameEvent endEnemyTurnEvent;
    [SerializeField] private AbilitySelectedEvent abilitySelectedEvent;
    
    private GameControls _controls;
    private IsometricGrid _grid;
    private CameraController _cameraController;
    private GridIndicators _gridIndicators;

    private void Awake() {
      _grid = IsometricGrid.Get();
      _cameraController = CameraController.Get();
      _gridIndicators = GridIndicators.Get(); 
      currentSelection.Reset();
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.TurnBasedEncounter.SetCallbacks(this);
      }
      _controls.TurnBasedEncounter.Enable();
      endEnemyTurnEvent.RegisterListener(OnStartPlayerTurn);
      abilitySelectedEvent.RegisterListener(OnAbilitySelected);
    }

    private void OnDisable() {
      _controls.TurnBasedEncounter.Disable();
      endEnemyTurnEvent.UnregisterListener(OnStartPlayerTurn);
      abilitySelectedEvent.UnregisterListener(OnAbilitySelected);
    }

    private void OnStartPlayerTurn() {
      _controls.TurnBasedEncounter.Enable();
    }

    private void OnAbilitySelected(UnitController actor, UnitAbility ability) {
      ability.OnSelected(actor, _gridIndicators);
    }
    
    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }

      var clickedObject = _cameraController.RaycastFromMousePosition().collider?.gameObject;
      var targetTile = _grid.TileAtScreenCoordinate(Mouse.current.position.ReadValue());

      if (currentSelection.TryGet(out var ability, out var unit)) {
        if (ability.TryExecute(unit, clickedObject, targetTile)) {
          return; 
        }
      }
      
      if (clickedObject != null) {
        objectClickedEvent.Raise(clickedObject.gameObject);
      }
    }
    
    public void OnPoint(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      
      var mousePosition = context.ReadValue<Vector2>();
      var hoveredObject = _cameraController.RaycastFromMousePosition().collider?.gameObject;
      var hoveredTile = _grid.TileAtScreenCoordinate(mousePosition);
      if (currentSelection.TryGet(out var ability, out var unit)) {
        ability.ShowIndicator(unit, hoveredObject, hoveredTile, _gridIndicators);
      }
      mouseHoverEvent.Raise(mousePosition);
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