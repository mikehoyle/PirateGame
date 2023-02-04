using System;
using System.Collections.Generic;
using System.Linq;
using CameraControl;
using Controls;
using HUD.Encounter;
using State;
using State.World;
using StaticConfig;
using Units;
using UnityEngine;

namespace Encounters {
  public class TurnBasedEncounterManager : MonoBehaviour {
    [SerializeField] private RawResourceScriptableObject lumberResource;
    [SerializeField] private RawResourceScriptableObject stoneResource;
    [SerializeField] private RawResourceScriptableObject foodResource;
    
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
    private IEnumerable<UnitEncounterManager> AliveUnits =>
        _unitsInEncounter.Where(unit => unit != null && unit.IsAlive);

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
      int id = 0;
      _unitsInEncounter = FindObjectsOfType<UnitController>()
          .Select(unit => {
            var encounterManager = unit.gameObject.AddComponent<UnitEncounterManager>();
            encounterManager.OnDeath += OnUnitDeath;
            encounterManager.OnBeginEncounter(id);
            id++;
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
          UnitsInEncounter = AliveUnits,
          CurrentTurnIndex = _currentUnitTurn,
          OnTurnEndedCallback = OnEndTurn,
      });
    }


    private void OnEndTurn() {
      var nextTurn = _currentUnitTurn; 
      do {
        nextTurn++;
        if (nextTurn >= _unitsInEncounter.Count) {
          _currentRound++;
          _hud.SetRound(_currentRound);
          nextTurn = 0;
        }
      } while (_unitsInEncounter[nextTurn] == null);

      OnNewUnitTurn(nextTurn);
    }

    private void OnUnitDeath(int unitId) {
      var killedUnit = _unitsInEncounter[unitId];
      if (killedUnit == null) {
        Debug.LogWarning("Killed unit already marked dead");
        return;
      }
      
      // Check for encounter end.

      var unitFaction = killedUnit.Faction;
      if (AliveUnits.Any(unit => unit.Faction == unitFaction)) {
        return;
      }
      
      // At this point, no units remain of the killed faction. End the encounter.
      EndEncounter(unitFaction);
    }
    private void EndEncounter(UnitFaction defeatedFaction) {
      _controls.Disable();
      foreach (var unit in AliveUnits) {
        unit.OnEndEncounter();
      }
      
      if (defeatedFaction == UnitFaction.PlayerParty) {
        _hud.EndEncounter(isVictory: false);
      } else {
        // TODO(P1): pull these from generated encounter config.
        GameState.State.Player.Inventory.AddQuantity(lumberResource, UnityEngine.Random.Range(10, 20));
        GameState.State.Player.Inventory.AddQuantity(stoneResource, UnityEngine.Random.Range(5, 10));
        GameState.State.Player.Inventory.AddQuantity(foodResource, UnityEngine.Random.Range(3, 8));
        
        // Clear the encounter from the map, do something else with these at some point?
        var coordinates = GameState.State.World.GetActiveTile().Coordinates;
        GameState.State.World.SetTile(coordinates.X, coordinates.Y, new OpenSeaTile());
        
        _hud.EndEncounter(isVictory: true);
      }
    }
  }
}