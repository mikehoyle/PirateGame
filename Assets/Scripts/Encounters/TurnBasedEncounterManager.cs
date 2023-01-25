using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Encounters {
  public class TurnBasedEncounterManager : MonoBehaviour {
    private int _currentRound;
    private List<UnitController> _unitsInEncounter = new();
    private int _currentUnitsTurn = 0;
    private EncounterHUD _hud;

    private void Start() {
      _hud = GameObject.FindWithTag(Tags.EncounterHUD).GetComponent<EncounterHUD>();
      _currentRound = 1;
      _unitsInEncounter = FindObjectsOfType<UnitController>().ToList();
      
      // Set to first units turn
      // Currently turn order is completely based on the order we found the components
      _currentUnitsTurn = 0;
      _hud.SetRound(_currentRound);
    }

    private void Update() {
      // TODO
    }

    /// <summary>
    /// PlayerInput event
    /// </summary>
    private void OnSelect(InputValue inputValue) {
      // TODO
      Debug.Log(inputValue.Get<Vector2>());
    }
  }
}