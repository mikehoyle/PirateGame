using System;
using Encounters.Grid;
using RuntimeVars.Encounters;
using State.Enemy;
using StaticConfig.Units;
using UnityEngine;

namespace Encounters.Enemies {
  public class EnemyUnitController : MonoBehaviour {
    [SerializeField] private EnemyUnitCollection enemiesInEncounter;
    private IsometricGrid _grid;

    public EnemyUnitEncounterState State { get; private set; }

    private void Awake() {
      _grid = IsometricGrid.Get();
    }

    private void OnEnable() {
      enemiesInEncounter.Add(this);
    }
    
    private void OnDisable() {
      enemiesInEncounter.Remove(this);
    }

    private void Update() {
      // TODO(P0): Prototype only
      transform.position = _grid.Grid.GetCellCenterWorld(State.position);
    }

    public void Init(EnemyUnitEncounterState state) {
      State = state;
      // TODO(P0): prototyping only, remove this
      State.resources = new[] {
          ExhaustibleResourceTracker.NewHpTracker(10), ExhaustibleResourceTracker.NewMovementTracker(5),
      };
    }
  }
}