using System.Collections.Generic;
using System.Linq;
using Common.Animation;
using Common.Events;
using Encounters;
using Optional;
using RuntimeVars;
using RuntimeVars.Encounters;
using State.Unit;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace Units {
  public class UnitController : EncounterActor {
    [SerializeField] private UnitCollection playerUnitsInEncounter;
    [SerializeField] private CurrentSelection currentSelection;
    
    private SpriteRenderer _selectedIndicator;

    public PlayerUnitMetadata Metadata => (PlayerUnitMetadata)EncounterState.metadata;

    public override UnitEncounterState EncounterState { get; protected set; }
    protected override EmptyGameEvent TurnPreStartEvent => encounterEvents.playerTurnPreStart;

    protected override void Awake() {
      base.Awake();
      _selectedIndicator = transform.Find("SelectedIndicator").GetComponent<SpriteRenderer>();
      _selectedIndicator.enabled = false;
    }

    private void Start() {
      GetComponentInChildren<CompositeDirectionalAnimator>().SetColorForFaction(EncounterState.faction);
    }

    protected override void OnEnable() {
      base.OnEnable();
      playerUnitsInEncounter.Add(this);
      encounterEvents.objectClicked.RegisterListener(OnObjectClicked);
      encounterEvents.abilityExecutionEnd.RegisterListener(OnAbilityEndExecution);
      encounterEvents.unitSelected.RegisterListener(OnUnitSelected);
      encounterEvents.trySelectAbilityByIndex.RegisterListener(TrySelectAbility);
    }

    protected override void OnDisable() {
      base.OnDisable();
      playerUnitsInEncounter.Remove(this);
      encounterEvents.objectClicked.UnregisterListener(OnObjectClicked);
      encounterEvents.abilityExecutionEnd.UnregisterListener(OnAbilityEndExecution);
      encounterEvents.trySelectAbilityByIndex.UnregisterListener(TrySelectAbility);
    }

    private void OnObjectClicked(GameObject clickedObject) {
      if (clickedObject == gameObject) {
        if (currentSelection.selectedUnit.Contains(this)) {
          // Unit clicked but already selected, do nothing.
          return;
        }
        
        currentSelection.selectedUnit = Option.Some<EncounterActor>(this);
        encounterEvents.unitSelected.Raise(this);
      }
    }

    private void OnUnitSelected(UnitController selectedUnit) {
      if (this == selectedUnit) {
        TrySelectAbility(0);
        _selectedIndicator.enabled = true;
        return;
      }
      _selectedIndicator.enabled = false;
    }

    private void OnAbilityEndExecution() {
      // If this unit is selected and just finished performing an ability,
      // Always go back to selecting the default ability
      if (currentSelection.selectedUnit.Contains(this)) {
        TrySelectAbility(0);
      }
    }

    // Here we assume the position is valid and just do the operation.
    public void SetShipPosition(Vector3Int position) {
      Position = position;
      Metadata.startingPosition = position;
    }

    private void TrySelectAbility(int index) {
      if (!currentSelection.selectedUnit.Contains(this)) {
        return;
      }

      var abilities = GetAllCapableAbilities();
      if (abilities.Count <= index) {
        return;
      }

      var selectedAbility = abilities[index];
      if (!selectedAbility.CanAfford(this)) {
        return;
      }
      currentSelection.SelectAbility(this, selectedAbility);
    }

    protected override void OnDeath() {
      if (currentSelection.selectedUnit.Contains(this)) {
        currentSelection.Clear();
      }
      playerUnitsInEncounter.Remove(this);
      // TODO(P1): play death animation.
      Destroy(gameObject);
    }
  }
}