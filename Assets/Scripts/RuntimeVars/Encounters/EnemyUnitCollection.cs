using System.Collections;
using System.Collections.Generic;
using Encounters.Enemies;
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

    public void Add(EnemyUnitController unit) {
      _units.Add(unit);
    }

    public void Remove(EnemyUnitController unit) {
      _units.Remove(unit);
    }
    
    public IEnumerator<EnemyUnitController> GetEnumerator() {
      return _units.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}