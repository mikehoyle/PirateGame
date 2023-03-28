using System.Collections.Generic;
using Common;
using Common.Animation;
using Common.Events;
using Encounters;
using Events;
using RuntimeVars;
using State.Encounter;
using State.Unit;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace Units {
  public class PlayerUnitController : EncounterActor {
    [SerializeField] private UnitCollection playerUnitsInEncounter;
    
    private Vector3Int _undoMovePosition;
    private int _undoMoveMp;

    public PlayerUnitMetadata Metadata => (PlayerUnitMetadata)EncounterState.metadata;
    public override UnitEncounterState EncounterState { get; protected set; }
    protected override GameEvent TurnPreStartEvent => Dispatch.Encounters.PlayerTurnPreStart;
    protected override GameEvent TurnEndEvent => Dispatch.Encounters.PlayerTurnEnd;

    public List<CollectableInstance> CollectablesAcquired { get; } = new();

    private void Start() {
      var animator = GetComponentInChildren<CompositeDirectionalAnimator>();
      animator.SetColor(Metadata.GetName());
      foreach (var equipment in Metadata.equipped.Values) {
        if (equipment.item.optionalEquippedSprite is not null) {
          animator.AddLayer(equipment.item.optionalEquippedSprite);
        }
      }
    }

    protected override void OnEnable() {
      base.OnEnable();
      playerUnitsInEncounter.Add(this);
      Dispatch.Encounters.AbilityExecutionEnd.RegisterListener(OnAbilityEndExecution);
      Dispatch.Encounters.UnitSelected.RegisterListener(OnUnitSelected);
      Dispatch.Encounters.TrySelectAbilityByIndex.RegisterListener(TrySelectAbility);
      Dispatch.Encounters.PlayerTurnPreStart.RegisterListener(OnTurnStart);
    }

    protected override void OnDisable() {
      base.OnDisable();
      playerUnitsInEncounter.Remove(this);
      Dispatch.Encounters.AbilityExecutionEnd.UnregisterListener(OnAbilityEndExecution);
      Dispatch.Encounters.UnitSelected.UnregisterListener(OnUnitSelected);
      Dispatch.Encounters.TrySelectAbilityByIndex.UnregisterListener(TrySelectAbility);
      Dispatch.Encounters.PlayerTurnPreStart.UnregisterListener(OnTurnStart);
    }

    private void OnTurnStart() {
      _undoMovePosition = Position;
      _undoMoveMp = EncounterState.GetResourceAmount(ExhaustibleResources.Instance.mp);
    }

    private void OnUnitSelected(EncounterActor selectedUnit) {
      if (selectedUnit != null && this == selectedUnit) {
        TrySelectAbility(0);
      }
    }

    private void OnAbilityEndExecution(EncounterActor actor, UnitAbility ability) {
      // If this unit is selected and just finished performing an ability,
      // Always go back to selecting the default ability
      if (currentSelection.SelectedUnit.Contains(this)) {
        if (currentSelection.SelectedAbility.TryGet(out var selectedAbility)
            && selectedAbility.CanAfford(this)) {
          // Keep current ability selected
        } else {
          TrySelectAbility(0); 
        }
      }
      
      // Reset our undo capability if:
      // - Anyone else does anything (including move).
      // - We do anything other than move.
      if (actor != this || (actor == this && ability is not MoveAbility)) {
        _undoMovePosition = Position;
        _undoMoveMp = EncounterState.GetResourceAmount(ExhaustibleResources.Instance.mp);
      }
    }

    public bool CanUndoMove() {
      return _undoMovePosition != Position && _undoMoveMp > 0;
    }

    public void UndoMoveIfEligible() {
      if (!CanUndoMove()) {
        return;
      }
      SetPosition(_undoMovePosition);
      // Reselect ability from new position.
      if (currentSelection.SelectedAbility.TryGet(out var ability)) {
        currentSelection.SelectAbility(this, ability);
      }
      EncounterState.TryGetResourceTracker(ExhaustibleResources.Instance.mp, out var mpTracker);
      mpTracker.current = _undoMoveMp;
    }

    // Here we assume the position is valid and just do the operation.
    public void SetPosition(Vector3Int position) {
      Mover.SnapToPosition(position);
    }

    private void TrySelectAbility(int index) {
      if (!currentSelection.SelectedUnit.Contains(this)) {
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
      if (currentSelection.SelectedUnit.Contains(this)) {
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