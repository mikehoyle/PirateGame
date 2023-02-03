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
using Object = System.Object;
using Random = UnityEngine.Random;

namespace Encounters {
  public class TurnBasedEncounterManager : MonoBehaviour {
    private int _currentRound;
    private List<UnitEncounterManager> _unitsInEncounter = new();
    private int _currentUnitTurn = 0;
    private EncounterHUD _hud;
    private CameraCursorMover _camera;
    private IsometricGrid _grid;
    private TargetingHintDisplay _targetingDisplay;
    private GameControls _controls;
    private ActionMenuController _actionMenu;
    private UnitAction _currentlySelectedAction;

    private UnitEncounterManager ActiveUnit => _unitsInEncounter[_currentUnitTurn];

    private void Awake() {
      _hud = GameObject.FindWithTag(Tags.EncounterHUD).GetComponent<EncounterHUD>();
      _actionMenu = ActionMenuController.Get();
      _camera = GetComponent<CameraCursorMover>();
      _grid = IsometricGrid.Get();
      _targetingDisplay = _grid.Grid.GetComponentInChildren<TargetingHintDisplay>();
      _controls = new GameControls();
      _currentRound = 1;
    }

    private void Start() {
      _unitsInEncounter = FindObjectsOfType<UnitController>()
          .Select(unit => {
            var encounterManager = unit.gameObject.AddComponent<UnitEncounterManager>();
            encounterManager.OnDeath += OnUnitDeath;
            return encounterManager;
          })
          .ToList();
      _hud.SetRound(_currentRound);
      // Set to first units turn
      // Currently turn order is completely arbitrarily based on the order we found the components
      OnNewUnitTurn(0);
    }
    
    private void OnEnable() {
      _controls.TurnBasedEncounter.Enable();
    }

    private void OnDisable() {
      _controls.TurnBasedEncounter.Disable();
    }
    
    private void OnNewUnitTurn(int unitIndex) {
      _currentUnitTurn = unitIndex;
      ActiveUnit.StartTurn(new UnitTurnContext {
          Controls = _controls.TurnBasedEncounter,
          TargetingDisplay = _targetingDisplay,
          ActionMenu = _actionMenu,
          Camera = _camera,
          UnitsInEncounter = _unitsInEncounter,
          CurrentTurnIndex = _currentUnitTurn,
          OnTurnEndedCallback = OnEndTurn,
      });
    }


    private void OnEndTurn() {
      var nextTurn = _currentUnitTurn + 1;
      if (nextTurn >= _unitsInEncounter.Count) {
        _currentRound++;
        _hud.SetRound(_currentRound);
        nextTurn = 0;
      }
      OnNewUnitTurn(nextTurn);
    }

    private void OnUnitDeath(object deadUnit, EventArgs _) {
      // TODO remove from array, maybe end encounter.
    }
  }
}