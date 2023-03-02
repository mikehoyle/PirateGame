using System.Collections.Generic;
using Common.Animation;
using Common.Events;
using Encounters.Grid;
using State.Unit;
using Units.Abilities;
using UnityEngine;
using EnemyUnitCollection = RuntimeVars.Encounters.EnemyUnitCollection;

namespace Encounters.Enemies {
  public class EnemyUnitController : EncounterActor {
    [SerializeField] private EnemyUnitCollection enemiesInEncounter;
    
    private DirectionalAnimator _animator;
    private List<UnitAbility> _abilities;
    private GridIndicators _gridIndicators;

    public override UnitEncounterState EncounterState { get; protected set; }
    protected override EmptyGameEvent TurnPreStartEvent => encounterEvents.enemyTurnPreStart;

    protected override void Awake() {
      base.Awake();
      _animator = GetComponent<DirectionalAnimator>();
      _gridIndicators = GridIndicators.Get();
    }

    protected override void OnEnable() {
      base.OnEnable();
      enemiesInEncounter.Add(this);
      encounterEvents.unitSelected.RegisterListener(OnUnitSelected);
    }

    protected override void OnDisable() {
      base.OnDisable();
      enemiesInEncounter.Remove(this);
      encounterEvents.unitSelected.UnregisterListener(OnUnitSelected);
    }
    protected override void InitInternal(UnitEncounterState encounterState) {
      // TODO(P1): remove this and generate it properly at encounter time (like player units do)
      EncounterState.resources = encounterState.metadata.GetEncounterTrackers();
      if (encounterState.metadata is not EnemyUnitMetadata enemyUnitMetadata) {
        Debug.LogWarning("Enemy units need to be initialized with enemy unit state");
        return;
      }
      _animator.SetSprite(enemyUnitMetadata.sprite);
    }

    private void OnUnitSelected(EncounterActor selectedUnit) {
      if (selectedUnit != null
          && this == selectedUnit
          && EncounterState.TryGetResourceTracker(exhaustibleResources.mp, out var movementRange)) {
        _gridIndicators.RangeIndicator.DisplayMovementRange(Position, movementRange.max);
      }
    }

    protected override void OnDeath() {
      enemiesInEncounter.Remove(this);
      PlayOneOffAnimation("death");
      Destroy(gameObject);
    }
  }
}