﻿using System.Collections.Generic;
using Common.Animation;
using RuntimeVars.Encounters;
using State.Unit;
using StaticConfig.Units;
using Terrain;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Enemies {
  public class EnemyUnitController : EncounterActor {
    [SerializeField] private EnemyUnitCollection enemiesInEncounter;
    [SerializeField] private UnitAbilitySet defaultAbilities;
    [SerializeField] private GameObject movementBlockerPrefab;
    
    private DirectionalAnimator _animator;
    private List<UnitAbility> _abilities;

    public override UnitEncounterState EncounterState { get; protected set; }

    protected override void Awake() {
      base.Awake();
      _animator = GetComponent<DirectionalAnimator>();
    }

    protected override void OnEnable() {
      base.OnEnable();
      enemiesInEncounter.Add(this);
      encounterEvents.enemyTurnStart.RegisterListener(OnNewRound);
    }

    protected override void OnDisable() {
      base.OnDisable();
      enemiesInEncounter.Remove(this);
      encounterEvents.enemyTurnStart.UnregisterListener(OnNewRound);
    }

    private void OnNewRound() {
      EncounterState.NewRound();
    }

    protected override void InitInternal(UnitEncounterState encounterState) {
      // TODO(P1): remove this and generate it properly at encounter time (like player units do)
      EncounterState.resources = encounterState.metadata.GetEncounterTrackers();
      ApplySize(encounterState.metadata.size);
      if (encounterState.metadata is not EnemyUnitMetadata enemyUnitMetadata) {
        Debug.LogWarning("Enemy units need to be initialized with enemy unit state");
        return;
      }
      _animator.SetSprite(enemyUnitMetadata.sprite);
    }

    private void ApplySize(Vector2Int size) {
      for (int x = 0; x < size.x; x++) {
        for (int y = 0; y < size.y; y++) {
          var movementBlocker = Instantiate(movementBlockerPrefab, transform);
          // Units are cell centered, so applying the blocker prefab at the cell "base", will actually
          // be center in world after adjusting for local coordinates.
          movementBlocker.transform.position =
              SceneTerrain.CellBaseWorldStatic(new Vector3Int(x, y, Position.z));
        }
      }
    }

    protected override void OnDeath() {
      enemiesInEncounter.Remove(this);
      PlayOneOffAnimation("death");
      Destroy(gameObject);
    }
  }
}