using System.Linq;
using Common;
using Controls;
using Encounters.Grid;
using IngameDebugConsole;
using RuntimeVars;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using State;
using State.World;
using Terrain;
using Units;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Managers {
  public class TurnBasedEncounterManager : EncounterInputReceiver {
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private IntegerVar currentRound;
    [SerializeField] private UnitCollection playerUnitsInEncounter;
    [SerializeField] private EnemyUnitCollection enemyUnitsInEncounter;
    [SerializeField] private CollectedResources collectedResources;
    
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
      encounterEvents.encounterStart.RegisterListener(OnEncounterStart);
    }

    private void OnDestroy() {
      encounterEvents.encounterStart.UnregisterListener(OnEncounterStart);
      currentSelection.Clear();
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.TurnBasedEncounter.SetCallbacks(this);
      }
      _controls.TurnBasedEncounter.Enable();
      encounterEvents.enemyTurnEnd.RegisterListener(OnEnemyTurnEnd);
      encounterEvents.abilitySelected.RegisterListener(OnAbilitySelected);
      encounterEvents.unitDeath.RegisterListener(OnUnitDeath);
      encounterEvents.encounterEnd.RegisterListener(OnEncounterEnd);
      
      DebugLogConsole.AddCommand("win", "Automatically win the encounter", DebugWin);
      DebugLogConsole.AddCommand("lose", "Automatically lose the encounter", DebugWin);
    }

    private void OnDisable() {
      _controls.TurnBasedEncounter.Disable();
      encounterEvents.enemyTurnEnd.UnregisterListener(OnEnemyTurnEnd);
      encounterEvents.abilitySelected.UnregisterListener(OnAbilitySelected);
      encounterEvents.unitDeath.UnregisterListener(OnUnitDeath);
      encounterEvents.encounterEnd.RegisterListener(OnEncounterEnd);
      
      DebugLogConsole.RemoveCommand("win");
      DebugLogConsole.RemoveCommand("lose");
    }

    private void OnEncounterStart() {
      enabled = true;
      collectedResources.Clear();
      encounterEvents.playerTurnPreStart.Raise();
      encounterEvents.playerTurnStart.Raise();
    }

    private void OnEnemyTurnEnd() {
      currentRound.Value += 1;
      _controls.TurnBasedEncounter.Enable();
      encounterEvents.playerTurnPreStart.Raise();
      encounterEvents.playerTurnStart.Raise();
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

    private void OnUnitDeath() {
      // Check for end conditions
      if (!playerUnitsInEncounter.Any()) {
        encounterEvents.encounterEnd.Raise(EncounterOutcome.PlayerDefeat);
        return;
      }

      if (!enemyUnitsInEncounter.Any()) {
        encounterEvents.encounterEnd.Raise(EncounterOutcome.PlayerVictory);
        return;
      }
    }
    
    private void OnEncounterEnd(EncounterOutcome outcome) {
      Debug.Log($"Encounter ending with outcome: {outcome}");
      if (outcome == EncounterOutcome.PlayerVictory) {
        var encounter = GameState.State.world.GetActiveTile().DownCast<EncounterWorldTile>(); 
        collectedResources.GiveResourcesToPlayer();
        foreach (var unit in GameState.State.player.roster) {
          unit.GrantXp(ExperienceCalculations.GetXpForVictoryInEncounter(encounter));
        }
        encounter.MarkDefeated();
      }
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
        encounterEvents.objectClicked.Raise(clickedObject.gameObject);
      }
    }
    
    protected override void OnPoint(Vector2 mousePosition) {
      if (_uiInteraction.isPlayerHoveringUi) {
        // Ignore UI-intended events
        return;
      }
      
      var hoveredTile = _terrain.TileAtScreenCoordinate(mousePosition);
      if (hoveredTile == _lastKnownHoveredTile) {
        // No need to update if the selected tile is the same
        return;
      }
      _lastKnownHoveredTile = hoveredTile;
      var hoveredObject = SceneTerrain.GetTileOccupant(hoveredTile);
      if (currentSelection.TryGet(out var ability, out var unit)) {
        ability.ShowIndicator(unit, currentSelection.abilitySource, hoveredObject, hoveredTile, _gridIndicators);
      }
      encounterEvents.mouseHover.Raise(hoveredTile);
    }

    protected override void OnTrySelectAction(int index) {
      encounterEvents.trySelectAbilityByIndex.Raise(index);
    }

    protected override void OnEndTurn() {
      currentSelection.Clear();
      _controls.TurnBasedEncounter.Disable();
      _gridIndicators.Clear();
      encounterEvents.playerTurnEnd.Raise();
      encounterEvents.playerTurnPreStart.Raise();
      encounterEvents.enemyTurnStart.Raise();
    }

    protected override void OnCancelSelection() {
      currentSelection.Clear();
      _gridIndicators.Clear();
      encounterEvents.unitSelected.Raise(null);
    }

    private void DebugWin() {
      encounterEvents.encounterEnd.Raise(EncounterOutcome.PlayerVictory);
    }
    
    private void DebugLose() {
      encounterEvents.encounterEnd.Raise(EncounterOutcome.PlayerDefeat);
    }
  }
}