using System.Collections.Generic;
using Common.Animation;
using Common.Events;
using Encounters;
using Events;
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
    protected override GameEvent TurnPreStartEvent => Dispatch.Encounters.PlayerTurnPreStart;

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
      Dispatch.Encounters.AbilityExecutionEnd.RegisterListener(OnAbilityEndExecution);
      Dispatch.Encounters.UnitSelected.RegisterListener(OnUnitSelected);
      Dispatch.Encounters.TrySelectAbilityByIndex.RegisterListener(TrySelectAbility);
    }

    protected override void OnDisable() {
      base.OnDisable();
      playerUnitsInEncounter.Remove(this);
      Dispatch.Encounters.AbilityExecutionEnd.UnregisterListener(OnAbilityEndExecution);
      Dispatch.Encounters.UnitSelected.UnregisterListener(OnUnitSelected);
      Dispatch.Encounters.TrySelectAbilityByIndex.UnregisterListener(TrySelectAbility);
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
    public void SetPosition(Vector3Int position) {
      Mover.SnapToPosition(position);
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
      
      foreach (var collectable in CollectablesAcquired) {
        if (collectable.isPrimaryObjective) {
          Dispatch.Encounters.SpawnCollectable.Raise(Position, collectable);
          return;
        }
      }
    }

    public void AddCollectable(CollectableInstance collectableInstance) {
      CollectablesAcquired.Add(collectableInstance);
    }
  }
}