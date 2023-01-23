using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;

namespace Encounters {
  public class TurnBasedEncounterManager : MonoBehaviour {
    private int _currentRound;
    private List<UnitController> _unitsInEncounter = new();
    
    private void Start() {
      _currentRound = 1;
      _unitsInEncounter = FindObjectsOfType<UnitController>().ToList();
      
      // Set to first units turn
      // Currently turn order is completely based on the order we found the components
      foreach (var unitController in _unitsInEncounter) {
        unitController.IsMyTurn = false;
      }
      _unitsInEncounter[0].IsMyTurn = true;
    }

    private void Update() {
      
    }
  }
}