using System.Collections.Generic;
using System.Linq;
using Encounters;
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
    [SerializeField] private Stat constitutionStat;
    [SerializeField] private Stat movementStat;
    
    private SpriteRenderer _selectedIndicator;

    public PlayerUnitMetadata Metadata => (PlayerUnitMetadata)EncounterState.metadata;

    public override UnitEncounterState EncounterState { get; protected set; }

    protected override void Awake() {
      base.Awake();
      _selectedIndicator = transform.Find("SelectedIndicator").GetComponent<SpriteRenderer>();
      _selectedIndicator.enabled = false;
    }

    private void Start() {
      GetComponentInChildren<AnimatedCompositeSprite>().SetColorForFaction(EncounterState.faction);
    }

    protected override void OnEnable() {
      base.OnEnable();
      playerUnitsInEncounter.Add(this);
      encounterEvents.objectClicked.RegisterListener(OnObjectClicked);
      encounterEvents.playerTurnStart.RegisterListener(OnNewRound);
      encounterEvents.abilityExecutionEnd.RegisterListener(OnAbilityEndExecution);
      encounterEvents.unitSelected.RegisterListener(OnUnitSelected);
    }

    protected override void OnDisable() {
      base.OnDisable();
      playerUnitsInEncounter.Remove(this);
      encounterEvents.objectClicked.UnregisterListener(OnObjectClicked);
      encounterEvents.playerTurnStart.UnregisterListener(OnNewRound);
      encounterEvents.abilityExecutionEnd.UnregisterListener(OnAbilityEndExecution);
    }
    
    public void Init(UnitEncounterState encounterState) {
      EncounterState = encounterState;
      Position = encounterState.position;
    }

    public List<UnitAbility> GetAllCapableAbilities() {
      var result = defaultAbilities.abilities.ToList();
      result.AddRange(Metadata.GetAbilities());
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

    // Here we assume the position is valid and just do the operation.
    public void SetShipPosition(Vector3Int position) {
      Position = position;
      Metadata.startingPosition = position;
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