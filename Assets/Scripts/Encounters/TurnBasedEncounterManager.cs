using CameraControl;
using Common.Events;
using Controls;
using Encounters.Grid;
using Pathfinding;
using RuntimeVars;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using Units;
using Units.Abilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Encounters {
  public class TurnBasedEncounterManager : EncounterInputReceiver {
    [SerializeField] private ObjectClickedEvent objectClickedEvent;
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private Vector3Event mouseHoverEvent;
    [SerializeField] private EmptyGameEvent endPlayerTurnEvent;
    [SerializeField] private EmptyGameEvent endEnemyTurnEvent;
    [SerializeField] private AbilitySelectedEvent abilitySelectedEvent;
    [SerializeField] private EmptyGameEvent beginAbilityExecutionEvent;
    [SerializeField] private EmptyGameEvent endAbilityExecutionEvent;
    [SerializeField] private IntegerVar currentRound;
    
    private GameControls _controls;
    private IsometricGrid _grid;
    private CameraController _cameraController;
    private GridIndicators _gridIndicators;
    private EncounterTerrain _terrain;

    private void Awake() {
      _grid = IsometricGrid.Get();
      _cameraController = CameraController.Get();
      _gridIndicators = GridIndicators.Get();
      _terrain = EncounterTerrain.Get();
      currentSelection.Reset();
      currentRound.Value = 1;
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.TurnBasedEncounter.SetCallbacks(this);
      }
      _controls.TurnBasedEncounter.Enable();
      endEnemyTurnEvent.RegisterListener(OnStartPlayerTurn);
      abilitySelectedEvent.RegisterListener(OnAbilitySelected);
      beginAbilityExecutionEvent.RegisterListener(OnBeginAbilityExecution);
      endAbilityExecutionEvent.RegisterListener(OnEndAbilityExecution);
    }

    private void OnDisable() {
      _controls.TurnBasedEncounter.Disable();
      endEnemyTurnEvent.UnregisterListener(OnStartPlayerTurn);
      abilitySelectedEvent.UnregisterListener(OnAbilitySelected);
      beginAbilityExecutionEvent.UnregisterListener(OnBeginAbilityExecution);
      endAbilityExecutionEvent.UnregisterListener(OnEndAbilityExecution);
    }

    private void OnStartPlayerTurn() {
      currentRound.Value += 1;
      _controls.TurnBasedEncounter.Enable();
    }

    private void OnAbilitySelected(UnitController actor, UnitAbility ability) {
      _gridIndicators.Clear();
      ability.OnSelected(actor, _gridIndicators);
    }

    private void OnBeginAbilityExecution() {
      _controls.TurnBasedEncounter.Disable();
    }

    private void OnEndAbilityExecution() {
      _controls.TurnBasedEncounter.Enable();
    }
    
    protected override void OnClick(Vector2 mousePosition) {
      var clickedObject = _cameraController.RaycastFromMousePosition().collider?.gameObject;
      var targetTile = _grid.TileAtScreenCoordinate(mousePosition);
      if (currentSelection.TryGet(out var ability, out var unit)) {
        if (ability.TryExecute(new UnitAbility.AbilityExecutionContext {
            Actor = unit,
            TargetedObject = clickedObject,
            TargetedTile = targetTile,
            Terrain =  _terrain,
            Indicators = _gridIndicators,
        })) {
          return; 
        }
      }
      
      if (clickedObject != null) {
        objectClickedEvent.Raise(clickedObject.gameObject);
      }
    }
    
    protected override void OnPoint(Vector2 mousePosition) {
      var hoveredObject = _cameraController.RaycastFromMousePosition().collider?.gameObject;
      var hoveredTile = _grid.TileAtScreenCoordinate(mousePosition);
      if (currentSelection.TryGet(out var ability, out var unit)) {
        ability.ShowIndicator(unit, hoveredObject, hoveredTile, _gridIndicators);
      }
      mouseHoverEvent.Raise(mousePosition);
    }

    protected override void OnTrySelectAction(int index) {
      if (currentSelection.TryGet(out _, out var unit)) {
        unit.TrySelectAbility(index);
      }
    }

    protected override void OnEndTurn() {
      _controls.TurnBasedEncounter.Disable();
      endPlayerTurnEvent.Raise();
    }
  }
}