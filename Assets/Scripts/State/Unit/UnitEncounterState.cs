using System.Collections.Generic;
using StaticConfig.Units;
using UnityEngine;

namespace State.Unit {
  [CreateAssetMenu(menuName = "State/UnitEncounterState")]
  public class UnitEncounterState : ScriptableObject {
    public int currentHp;
    public List<UnitAbility> capableAbilities;
    public int remainingMovement;
    public Vector3Int position;
    
    public bool IsAlive => currentHp > 0;

    public void NewEncounter(UnitState unitState, Vector3Int positionOffset) {
      currentHp = unitState.maxHp;
      capableAbilities = new();
      remainingMovement = unitState.movementRange;
      position = unitState.startingPosition + positionOffset;
    }
  }
}