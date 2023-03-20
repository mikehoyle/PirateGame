using System.Collections.Generic;
using Common.Animation;
using Common.Events;
using Encounters;
using RuntimeVars;
using State.Encounter;
using State.Unit;
using UnityEngine;

namespace Units {
  public class PlayerUnitController : EncounterActor {
    [SerializeField] private UnitCollection playerUnitsInEncounter;
    
    private SpriteRenderer _selectedIndicator;

    public PlayerUnitMetadata Metadata => (PlayerUnitMetadata)EncounterState.metadata;
    public override UnitEncounterState EncounterState { get; protected set; }
    protected override EmptyGameEvent TurnPreStartEvent => encounterEvents.playerTurnPreStart;

    public List<CollectableInstance> CollectablesAcquired { get; } = new();

    protected override void Awake() {
      base.Awake();
      _selectedIndicator = transform.Find("SelectedIndicator").GetComponent<SpriteRenderer>();
      _selectedIndicator.enabled = false;
    }

    private void Start() {
      GetComponentInChildren<CompositeDirectionalAnimator>().SetColor(Metadata.GetName());
    }

    protected override void OnEnable() {
      base.OnEnable();
      playerUnitsInEncounter.Add(this);
      encounterEvents.abilityExecutionEnd.RegisterListener(OnAbilityEndExecution);
      encounterEvents.unitSelected.RegisterListener(OnUnitSelected);
      encounterEvents.trySelectAbilityByIndex.RegisterListener(TrySelectAbility);
    }

    protected override void OnDisable() {
      base.OnDisable();
      playerUnitsInEncounter.Remove(this);
      encounterEvents.abilityExecutionEnd.UnregisterListener(OnAbilityEndExecution);
      encounterEvents.unitSelected.UnregisterListener(OnUnitSelected);
      encounterEvents.trySelectAbilityByIndex.UnregisterListener(TrySelectAbility);
    }

    private void OnUnitSelected(EncounterActor selectedUnit) {
      if (selectedUnit != null && this == selectedUnit) {
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

    public void AddCollectable(CollectableInstance collectableInstance) {
      CollectablesAcquired.Add(collectableInstance);
    }
  }
}