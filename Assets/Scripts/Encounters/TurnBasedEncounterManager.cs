using System.Linq;
using CameraControl;
using Common;
using Controls;
using Encounters.Grid;
using RuntimeVars;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using State;
using State.World;
using Terrain;
using Units;
using Units.Abilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Encounters {
  public class TurnBasedEncounterManager : EncounterInputReceiver {
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private IntegerVar currentRound;
    [SerializeField] private UnitCollection playerUnitsInEncounter;
    [SerializeField] private EnemyUnitCollection enemyUnitsInEncounter;
    [SerializeField] private CollectedResources collectedResources;
    
    private GameControls _controls;
    private CameraController _cameraController;
    private GridIndicators _gridIndicators;
    private SceneTerrain _terrain;
    private LayerMask _unitInteractionLayer;
    private UiInteractionTracker _uiInteraction;

    private void Awake() {
      _cameraController = CameraController.Get();
      _gridIndicators = GridIndicators.Get();
      _terrain = SceneTerrain.Get();
      _uiInteraction = GetComponent<UiInteractionTracker>();
      currentSelection.Clear();
      currentRound.Value = 1;
      encounterEvents.encounterStart.RegisterListener(OnEncounterStart);
    }

    private void Start() {
      _unitInteractionLayer = LayerMask.GetMask("Clickable");
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
      encounterEvents.abilityExecutionStart.RegisterListener(OnBeginAbilityExecution);
      encounterEvents.abilityExecutionEnd.RegisterListener(OnEndAbilityExecution);
      encounterEvents.unitDeath.RegisterListener(OnUnitDeath);
      encounterEvents.encounterEnd.RegisterListener(OnEncounterEnd);
    }

    private void OnDisable() {
      _controls.TurnBasedEncounter.Disable();
      encounterEvents.enemyTurnEnd.UnregisterListener(OnEnemyTurnEnd);
      encounterEvents.abilitySelected.UnregisterListener(OnAbilitySelected);
      encounterEvents.abilityExecutionStart.UnregisterListener(OnBeginAbilityExecution);
      encounterEvents.abilityExecutionEnd.UnregisterListener(OnEndAbilityExecution);
      encounterEvents.unitDeath.UnregisterListener(OnUnitDeath);
      encounterEvents.encounterEnd.RegisterListener(OnEncounterEnd);
    }

    private void OnEncounterStart() {
      enabled = true;
      collectedResources.Clear();
      encounterEvents.playerTurnStart.Raise();
    }

    private void OnEnemyTurnEnd() {
      currentRound.Value += 1;
      _controls.TurnBasedEncounter.Enable();
      encounterEvents.playerTurnStart.Raise();
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
        collectedResources.GiveResourcesToPlayer();
        foreach (var unit in GameState.State.player.roster) {
          unit.GrantXp(ExperienceCalculations.GetXpForVictoryInEncounter(
              GameState.State.world.GetActiveTile().DownCast<EncounterTile>()));
        }
      }
      enabled = false;
    } 

    protected override void OnClick(Vector2 mousePosition) {
      if (_uiInteraction.isPlayerHoveringUi) {
        // Ignore UI-intended events
        return;
      }
      
      var clickedObject = _cameraController.RaycastFromMousePosition(_unitInteractionLayer).collider?.gameObject;
      var targetTile = _terrain.TileAtScreenCoordinate(mousePosition);
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
        encounterEvents.objectClicked.Raise(clickedObject.gameObject);
      }
    }
    
    protected override void OnPoint(Vector2 mousePosition) {
      if (_uiInteraction.isPlayerHoveringUi) {
        // Ignore UI-intended events
        return;
      }
      
      var hoveredObject = _cameraController.RaycastFromMousePosition(_unitInteractionLayer).collider?.gameObject;
      var hoveredTile = _terrain.TileAtScreenCoordinate(mousePosition);
      if (currentSelection.TryGet(out var ability, out var unit)) {
        ability.ShowIndicator(unit, hoveredObject, hoveredTile, _gridIndicators);
      }
      encounterEvents.mouseHover.Raise(mousePosition);
    }

    protected override void OnTrySelectAction(int index) {
      if (currentSelection.TryGet(out _, out var unit)) {
        if (unit is UnitController playerUnit) {
          playerUnit.TrySelectAbility(index);  
        }
      }
    }

    protected override void OnEndTurn() {
      currentSelection.Clear();
      _controls.TurnBasedEncounter.Disable();
      _gridIndicators.Clear();
      encounterEvents.playerTurnEnd.Raise();
      encounterEvents.enemyTurnStart.Raise();
    }
  }
}