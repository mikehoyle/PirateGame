using System;
using System.Collections;
using System.Collections.Generic;
using State;
using Units;
using UnityEngine;

namespace RuntimeVars {
  [CreateAssetMenu(menuName = "Encounters/UnitCollection")]
  public class UnitCollection : ScriptableObject, IEnumerable<UnitController> {
    private List<UnitController> _units;

    private void Awake() {
      _units = new();
    }
    
    private void OnDestroy() {
      _units = new();
    }

    public void Add(UnitController unit) {
      _units.Add(unit);
    }

    public void Remove(UnitController unit) {
      _units.Remove(unit);
    }
    
    public IEnumerator<UnitController> GetEnumerator() {
      return _units.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}