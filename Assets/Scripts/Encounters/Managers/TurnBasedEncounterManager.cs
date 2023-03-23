using Common;
using Controls;
using Encounters.Grid;
using Events;
using IngameDebugConsole;
using RuntimeVars;
using RuntimeVars.Encounters;
using Terrain;
using Units;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Managers {
  public class TurnBasedEncounterManager : EncounterInputReceiver {
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private IntegerVar currentRound;
    [SerializeField] private UnitCollection playerUnitsInEncounter;
    [SerializeField] private EnemyUnitCollection enemyUnitsInEncounter;
    
    private GameControls _controls;
    private GridIndicators _gridIndicators;
    private SceneTerrain _terrain;
    private UiInteractionTracker _uiInteraction;
    private Vector3Int _lastKnownHoveredTile = new(int.MaxValue, int.MaxValue, 0);

    private void Awake() {
      _gridIndicators = GridIndicators.Get();
      _terrain = SceneTerrain.Get();
      _uiInteraction = GetComponent<UiInteractionTracker>();
      currentSelection.Clear();
      currentRound.Value = 1;
      Dispatch.Encounters.EncounterStart.RegisterListener(OnEncounterStart);
    }

    private void OnDestroy() {
      Dispatch.Encounters.EncounterStart.UnregisterListener(OnEncounterStart);
      currentSelection.Clear();
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.TurnBasedEncounter.SetCallbacks(this);
      }
      _controls.TurnBasedEncounter.Enable();
      Dispatch.Encounters.EnemyTurnEnd.RegisterListener(OnEnemyTurnEnd);
      Dispatch.Encounters.AbilitySelected.RegisterListener(OnAbilitySelected);
      Dispatch.Encounters.EncounterEnd.RegisterListener(OnEncounterEnd);
      Dispatch.Encounters.MouseHover.RegisterListener(OnMouseHover);
      Dispatch.Common.DialogueStart.RegisterListener(OnDialogueStart);
      Dispatch.Common.DialogueEnd.RegisterListener(OnDialogueEnd);
      
      DebugLogConsole.AddCommand("win", "Automatically win the encounter", DebugWin);
      DebugLogConsole.AddCommand("lose", "Automatically lose the encounter", DebugLose);
    }

    private void OnDisable() {
      _controls?.TurnBasedEncounter.Disable();
      Dispatch.Encounters.EnemyTurnEnd.UnregisterListener(OnEnemyTurnEnd);
      Dispatch.Encounters.AbilitySelected.UnregisterListener(OnAbilitySelected);
      Dispatch.Encounters.EncounterEnd.UnregisterListener(OnEncounterEnd);
      Dispatch.Encounters.MouseHover.UnregisterListener(OnMouseHover);
      Dispatch.Common.DialogueStart.UnregisterListener(OnDialogueStart);
      Dispatch.Common.DialogueEnd.UnregisterListener(OnDialogueEnd);
      
      DebugLogConsole.RemoveCommand("win");
      DebugLogConsole.RemoveCommand("lose");
    }

    private void OnEncounterStart() {
      enabled = true;
      Dispatch.Encounters.PlayerTurnPreStart.Raise();
      Dispatch.Encounters.PlayerTurnStart.Raise();
    }

    private void OnEnemyTurnEnd() {
      currentRound.Value += 1;
      _controls.TurnBasedEncounter.Enable();
      Dispatch.Encounters.PlayerTurnPreStart.Raise();
      Dispatch.Encounters.PlayerTurnStart.Raise();
    }

    private void OnAbilitySelected(PlayerUnitController actor, UnitAbility ability, Vector3Int source) {
      _gridIndicators.Clear();
      ability.OnSelected(actor, _gridIndicators, source);
      ability.ShowIndicator(
          actor, source, SceneTerrain.GetTileOccupant(_lastKnownHoveredTile), _lastKnownHoveredTile, _gridIndicators);
    }

    private void OnBeginAbilityExecution() {
      _controls.TurnBasedEncounter.Disable();
    }

    private void OnEndAbilityExecution() {
      _controls.TurnBasedEncounter.Enable();
    }
    
    private void OnEncounterEnd(EncounterOutcome outcome) {
      enabled = false;
    }

    protected override void OnClick(Vector2 mousePosition) {
      if (_uiInteraction.isPlayerHoveringUi) {
        // Ignore UI-intended events
        return;
      }
      
      var targetTile = _terrain.TileAtScreenCoordinate(mousePosition);
      var clickedObject = SceneTerrain.GetTileOccupant(targetTile);
      if (currentSelection.TryGet(out var ability, out var unit)) {
        var executionContext = new UnitAbility.AbilityExecutionContext {
            Actor = unit,
            Source = currentSelection.abilitySource,
            TargetedObject = clickedObject,
            TargetedTile = targetTile,
            Terrain = _terrain,
            Indicators = _gridIndicators,
        };
        var attemptedExecution = ability.TryExecute(executionContext, OnEndAbilityExecution);
        if (attemptedExecution.TryGet(out var abilityExecution)) {
          OnBeginAbilityExecution();
          StartCoroutine(abilityExecution);
          return;
        }
      }
      
      if (clickedObject != null) {
        Dispatch.Common.ObjectClicked.Raise(clickedObject.gameObject);
      }
    }

    private void OnMouseHover(Vector3Int hoveredTile) {
      _lastKnownHoveredTile = hoveredTile;
      var hoveredObject = SceneTerrain.GetTileOccupant(hoveredTile);
      if (currentSelection.TryGet(out var ability, out var unit)) {
        ability.ShowIndicator(unit, currentSelection.abilitySource, hoveredObject, hoveredTile, _gridIndicators);
      }
    }

    protected override void OnTrySelectAction(int index) {
      Dispatch.Encounters.TrySelectAbilityByIndex.Raise(index);
    }

    protected override void OnEndTurn() {
      currentSelection.Clear();
      _controls.TurnBasedEncounter.Disable();
      _gridIndicators.Clear();
      Dispatch.Encounters.PlayerTurnEnd.Raise();
      Dispatch.Encounters.EnemyTurnPreStart.Raise();
      Dispatch.Encounters.EnemyTurnStart.Raise();
    }

    protected override void OnCancelSelection() {
      currentSelection.Clear();
      _gridIndicators.Clear();
      Dispatch.Encounters.UnitSelected.Raise(null);
    }

    private void DebugWin() {
      Dispatch.Encounters.EncounterEnd.Raise(EncounterOutcome.PlayerVictory);
    }
    
    private void DebugLose() {
      Dispatch.Encounters.EncounterEnd.Raise(EncounterOutcome.PlayerDefeat);
    }

    private void OnDialogueStart() {
      _controls.TurnBasedEncounter.Disable();
    }

    private void OnDialogueEnd() {
      _controls.TurnBasedEncounter.Enable();
    }
  }
}