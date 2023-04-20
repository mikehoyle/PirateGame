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
    }

    private void OnEnable() {
      GameInput.Controls.TurnBasedEncounter.SetCallbacks(this);
      
      Dispatch.Encounters.EnemyTurnEnd.RegisterListener(OnEnemyTurnEnd);
      Dispatch.Encounters.AbilitySelected.RegisterListener(OnAbilitySelected);
      Dispatch.Encounters.EncounterEnd.RegisterListener(OnEncounterEnd);
      Dispatch.Encounters.MouseHover.RegisterListener(OnMouseHover);
      Dispatch.Encounters.PlayerTurnEndRequest.RegisterListener(OnRequestEndTurn);
      
      DebugLogConsole.AddCommand("win", "Automatically win the encounter", DebugWin);
      DebugLogConsole.AddCommand("lose", "Automatically lose the encounter", DebugLose);
      DebugLogConsole.AddCommand<int, int>("tp", "Teleport selected unit to coordinates", DebugTeleport);
    }

    private void OnDisable() {
      GameInput.Controls.TurnBasedEncounter.RemoveCallbacks(this);
      
      Dispatch.Encounters.EnemyTurnEnd.UnregisterListener(OnEnemyTurnEnd);
      Dispatch.Encounters.AbilitySelected.UnregisterListener(OnAbilitySelected);
      Dispatch.Encounters.EncounterEnd.UnregisterListener(OnEncounterEnd);
      Dispatch.Encounters.MouseHover.UnregisterListener(OnMouseHover);
      Dispatch.Encounters.PlayerTurnEndRequest.UnregisterListener(OnRequestEndTurn);
      
      DebugLogConsole.RemoveCommand("win");
      DebugLogConsole.RemoveCommand("lose");
      DebugLogConsole.RemoveCommand("tp");
    }

    private void Update() {
      if (currentSelection.TryGet(out var ability, out var unit)) {
        ability.ShowIndicator(unit, currentSelection.AbilitySource, _lastKnownHoveredTile, _gridIndicators);
      }
    }

    private void OnEncounterStart() {
      enabled = true;
      Dispatch.Encounters.PlayerTurnPreStart.Raise();
      Dispatch.Encounters.PlayerTurnStart.Raise();
    }

    private void OnEnemyTurnEnd() {
      currentRound.Value += 1;
      Dispatch.Encounters.PlayerTurnPreStart.Raise();
      Dispatch.Encounters.PlayerTurnStart.Raise();
    }

    private void OnAbilitySelected(PlayerUnitController actor, UnitAbility ability, Vector3Int source) {
      _gridIndicators.Clear();
      ability.OnSelected(actor, _gridIndicators, source);
      ability.ShowIndicator(
          actor, source, _lastKnownHoveredTile, _gridIndicators);
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
      if (currentSelection.TryGet(out var ability, out var unit)) {
        var executionContext = new UnitAbility.AbilityExecutionContext {
            Actor = unit,
            Source = currentSelection.AbilitySource,
            TargetedTile = targetTile,
            Terrain = _terrain,
            Indicators = _gridIndicators,
        };
        var attemptedExecution = ability.TryExecute(executionContext);
        if (attemptedExecution.TryGet(out var abilityExecution)) {
          StartCoroutine(abilityExecution);
          return;
        }
      }

      foreach (var clickedObjects in SceneTerrain.GetAllTileOccupants(targetTile)) {
        Dispatch.Common.ObjectClicked.Raise(clickedObjects);
      }
    }

    private void OnMouseHover(Vector3Int hoveredTile) {
      _lastKnownHoveredTile = hoveredTile;
      if (currentSelection.TryGet(out var ability, out var unit)) {
        ability.ShowIndicator(unit, currentSelection.AbilitySource, hoveredTile, _gridIndicators);
      }
    }

    protected override void OnTrySelectAction(int index) {
      Dispatch.Encounters.TrySelectAbilityByIndex.Raise(index);
    }

    protected override void OnEndTurnKey() {
      Dispatch.Encounters.PlayerTurnEndRequest.Raise();
    }

    private void OnRequestEndTurn() {
      currentSelection.Clear();
      Dispatch.Encounters.UnitSelected.Raise(null);
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

    private void DebugTeleport(int x, int y) {
      if (currentSelection.TryGetUnit<PlayerUnitController>(out var unit)) {
        unit.SetPosition(new Vector3Int(x, y, 0));
      }
    }
  }
}