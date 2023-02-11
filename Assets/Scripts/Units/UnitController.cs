using System.Collections.Generic;
using System.Linq;
using Encounters;
using JetBrains.Annotations;
using Optional;
using RuntimeVars;
using RuntimeVars.Encounters;
using State.Unit;
using StaticConfig.Units;
using Units.Abilities;
using Units.Rendering;
using UnityEngine;

namespace Units {
  public class UnitController : EncounterActor {
    [SerializeField] private UnitCollection playerUnitsInEncounter;
    [SerializeField] private UnitAbilitySet defaultAbilities;
    [SerializeField] private CurrentSelection currentSelection;
    
    [CanBeNull] private UnitPlacementManager _placementManager;
    
    public UnitState State { get; private set; }

    public override UnitEncounterState EncounterState {
      get => State.encounterState;
      protected set => State.encounterState = value;
    }

    protected override void OnEnable() {
      base.OnEnable();
      playerUnitsInEncounter.Add(this);
      encounterEvents.objectClicked.RegisterListener(OnObjectClicked);
      encounterEvents.playerTurnStart.RegisterListener(OnNewRound);
      encounterEvents.abilityExecutionEnd.RegisterListener(OnAbilityEndExecution);
    }

    protected override void OnDisable() {
      base.OnDisable();
      playerUnitsInEncounter.Remove(this);
      encounterEvents.objectClicked.UnregisterListener(OnObjectClicked);
      encounterEvents.playerTurnStart.UnregisterListener(OnNewRound);
      encounterEvents.abilityExecutionEnd.UnregisterListener(OnAbilityEndExecution);
    }

    private void Start() {
      GetComponentInChildren<AnimatedCompositeSprite>().SetColorForFaction(EncounterState.faction);
    }

    public void Init(UnitState state) {
      Init(state, Vector3Int.zero);
    }

    public void Init(UnitState state, Vector3Int positionOffset) {
      State = state;
      // TODO(P0): prototyping only, remove this
      state.encounterState.resources = new[] {
          ExhaustibleResourceTracker.NewHpTracker(20),
          ExhaustibleResourceTracker.NewActionPointsTracker(2),
          ExhaustibleResourceTracker.NewMovementTracker(8),
      };
      state.encounterState.NewEncounter(state.startingPosition + positionOffset);
      Position = State.startingPosition + positionOffset;
    }

    public List<UnitAbility> GetAllCapableAbilities() {
      var result = defaultAbilities.abilities.ToList();
      result.AddRange(State.GetAbilities());
      return result;
    }

    private void OnObjectClicked(GameObject clickedObject) {
      if (clickedObject == gameObject) {
        if (currentSelection.selectedUnit.Contains(this)) {
          // Unit clicked but already selected, do nothing.
          return;
        }
        
        currentSelection.selectedUnit = Option.Some<EncounterActor>(this);
        encounterEvents.unitSelected.Raise(this);
        TrySelectAbility(0);
      }
    }

    private void OnNewRound() {
      EncounterState.NewRound();
    }

    private void OnAbilityEndExecution() {
      // If this unit is selected and just finished performing an ability,
      // Always go back to selecting the default ability
      if (currentSelection.selectedUnit.Contains(this)) {
        TrySelectAbility(0);
      }
    }

    public void TrySelectAbility(int index) {
      var abilities = GetAllCapableAbilities();
      if (abilities.Count <= index) {
        return;
      }

      var selectedAbility = abilities[index];
      if (!selectedAbility.CanAfford(this)) {
        return;
      }
      currentSelection.selectedAbility = Option.Some(selectedAbility);
      encounterEvents.abilitySelected.Raise(this, selectedAbility);
    }
  }
}