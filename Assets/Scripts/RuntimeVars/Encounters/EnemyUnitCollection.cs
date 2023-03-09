using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Encounters.Enemies;
using State.Unit;
using UnityEngine;

namespace RuntimeVars.Encounters {
  [CreateAssetMenu(menuName = "Encounters/EnemyUnitCollection")]
  public class EnemyUnitCollection : ScriptableObject, IEnumerable<EnemyUnitController> {
    private List<EnemyUnitController> _units;

    private void Awake() {
      _units = new();
    }

    private void OnDestroy() {
      _units = new();
    }

    public int Count => _units.Count;

    public void Add(EnemyUnitController unit) {
      _units.Add(unit);
    }

    public void Remove(EnemyUnitController unit) {
      _units.Remove(unit);
    }

    public IEnumerable<EnemyUnitController> EnumerateByTurnPriority() {
      return _units.OrderBy(unit => ((EnemyUnitMetadata)unit.EncounterState.metadata).turnPriority);
    }

    public IEnumerator<EnemyUnitController> GetEnumerator() {
      return _units.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}