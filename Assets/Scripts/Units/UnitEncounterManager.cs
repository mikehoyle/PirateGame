using System;
using System.Collections.Generic;
using Controls;
using Encounters;
using JetBrains.Annotations;
using Pathfinding;
using State;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Units {
  [Serializable]
  public class UnitEncounterManager : MonoBehaviour, GameControls.ITurnBasedEncounterActions {
    private UnitController _unit;
    private EncounterTerrain _terrain;
    private UnitAction _currentlySelectedAction;
    private IsometricGrid _grid;
    [CanBeNull] private UnitTurnContext _turnContext;

    public delegate void OnDeathHandler(int encounterId);
    public event OnDeathHandler OnDeath; 

    public int CurrentHp { get; set; }
    public List<UnitAction> CapableActions { get; } = new();
    public List<UnitAction> AvailableActions { get; private set; } = new();
    public int RemainingMovement { get; set; }
    public bool IsMyTurn { get; private set; }
    public int EncounterId { get; private set; }
    public UnitFaction Faction => _unit.State.Faction;
    public Vector3Int Position => _unit.Position;
    public bool IsAlive => CurrentHp > 0;

    private void Awake() {
      _unit = GetComponent<UnitController>();
      _grid = IsometricGrid.Get();
      _terrain = EncounterTerrain.Get();
      _terrain.SetEnabled(_unit.Position, false);
      CurrentHp = _unit.State.MaxHp;
      GetComponentInChildren<HpBarController>().Init(this, _unit.State.MaxHp);
      AddAvailableActions();
    }

    private void AddAvailableActions() {
      CapableActions.Add(UnitAction.Move);
      CapableActions.Add(UnitAction.AttackMelee);
      CapableActions.Add(UnitAction.EndTurn);
    }

    public void StartTurn(UnitTurnContext context) {
      _turnContext = context;
      // Always mark self traversable, so unit can move out of their own tile
      _terrain.SetEnabled(_unit.Position, true);
      context.Camera.MoveCursorDirectly(_unit.WorldPosition);
      RemainingMovement = _unit.State.MovementRange;
      AvailableActions = new(CapableActions);
      // Display action options, and default select the first
      context.ActionMenu.SetActiveUnit(this);
      SelectAction(AvailableActions[0]);
      
      // Take control of controls handling
      context.Controls.SetCallbacks(this);
      IsMyTurn = true;
    }

    private void EndTurn() {
      _terrain.SetEnabled(_unit.Position, false);
      IsMyTurn = false;
      _turnContext?.Controls.SetCallbacks(null);
      _turnContext?.OnTurnEndedCallback();
      _turnContext = null;
    }

    public void OnBeginEncounter(int encounterId) {
      EncounterId = encounterId;
    }

    public void OnEndEncounter() {
      IsMyTurn = false;
      _turnContext?.Controls.SetCallbacks(null);
      _turnContext = null;
    }

    public bool IsUnitEnemy(UnitEncounterManager unit) {
      return Faction != unit.Faction;
    }

    public void TakeDamage(int damageAmount) {
      CurrentHp -= damageAmount;
      if (CurrentHp <= 0) {
        OnDeath?.Invoke(EncounterId);
        // TODO death animation
        Destroy(gameObject);
      }
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
      foreach (var unit in _turnContext!.UnitsInEncounter) {
        if (unit.Position == gridCell && IsUnitEnemy(unit)) {
          // TODO(P0): Make attacking not completely random;
          unit.TakeDamage(Random.Range(3, 6));
          AvailableActions.Remove(UnitAction.AttackMelee);
          SelectAction(UnitAction.None);
          return;
        }
      }
    }

    private void AttemptMove(Vector3Int gridCell) {
      var path = _terrain.GetPath(_unit.Position, gridCell);

      if (path != null && path.Count - 1 > RemainingMovement) {
        return;
      }
      
      var formerPosition = _unit.Position;
      if (_unit.MoveAlongPath(path, OnMoveComplete)) {
        _terrain.SetEnabled(formerPosition, true);
        RemainingMovement -= (path!.Count - 1);
        _turnContext!.TargetingDisplay.ClearAll();
        _turnContext.Controls.Disable();
      }
    }

    private void OnMoveComplete() {
      _turnContext!.Camera.MoveCursorDirectly(_unit.WorldPosition);
      if (RemainingMovement <= 0) {
        AvailableActions.Remove(UnitAction.Move);
        _currentlySelectedAction = UnitAction.None;
      }
      SelectAction(_currentlySelectedAction);
      _turnContext.Controls.Enable();
    }

    public void OnPoint(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }

      // TODO(P1): All these switch-cases are ugly and not maintainable. Make a better way. 
      switch (_currentlySelectedAction) {
        case UnitAction.Move:
          _turnContext!.TargetingDisplay
              .HandleMouseHover(context.ReadValue<Vector2>(), _unit.Position, RemainingMovement);
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
      if (CapableActions.Count < index) {
        return;
      }
      var action = CapableActions[index - 1];
      if (!AvailableActions.Contains(action)) {
        return;
      }
      SelectAction(action);
    }

    private void SelectAction(UnitAction unitAction) {
      if (_turnContext == null) {
        return;
      }
      
      _currentlySelectedAction = unitAction;
      _turnContext!.TargetingDisplay.ClearAll();

      switch (unitAction) {
        case UnitAction.Move:
          _turnContext.TargetingDisplay.DisplayMovementPossibilities(_unit.Position, RemainingMovement);
          return;
        case UnitAction.AttackMelee:
          _turnContext.TargetingDisplay.DisplayAttackPossibilities(_unit.Position);
          return;
        case UnitAction.EndTurn:
          EndTurn();
          return;
      }
    }
  }
}