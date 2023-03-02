using System;
using System.Collections;
using System.Collections.Generic;
using State;
using Units;
using UnityEngine;

namespace RuntimeVars {
  [CreateAssetMenu(menuName = "Encounters/UnitCollection")]
  public class UnitCollection : ScriptableObject, IEnumerable<PlayerUnitController> {
    private List<PlayerUnitController> _units;

    private void Awake() {
      _units = new();
    }
    
    private void OnDestroy() {
      _units = new();
    }

    public void Add(PlayerUnitController unit) {
      _units.Add(unit);
    }

    public void Remove(PlayerUnitController unit) {
      _units.Remove(unit);
    }
    
    public IEnumerator<PlayerUnitController> GetEnumerator() {
      return _units.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}