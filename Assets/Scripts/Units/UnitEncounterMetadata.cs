using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units {
  [Serializable]
  public class UnitEncounterMetadata {
    private readonly UnitController _unit;
    
    public Vector3Int Position { get; set; }
    public int CurrentHp { get; set; }
    public List<UnitAction> CapableActions { get; } = new();
    public List<UnitAction> AvailableActions { get; private set; } = new();
    public int RemainingMovement { get; set; }
    public bool IsMyTurn { get; private set; }

    public UnitEncounterMetadata(UnitController unit, Vector3Int position) {
      _unit = unit;
      CurrentHp = unit.State.MaxHp;
      Position = position;
      AddAvailableActions();
    }

    private void AddAvailableActions() {
      CapableActions.Add(UnitAction.Move);
      CapableActions.Add(UnitAction.AttackMelee);
      CapableActions.Add(UnitAction.EndTurn);
    }

    public void StartTurn() {
      RemainingMovement = _unit.State.MovementRange;
      AvailableActions = new(CapableActions);
      IsMyTurn = true;
    }

    public void EndTurn() {
      IsMyTurn = false;
    }
  }
}