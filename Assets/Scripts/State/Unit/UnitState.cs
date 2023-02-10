using System;
using System.Collections.Generic;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace State.Unit {
  // TODO(P1): Create reasonable separation for far-less-capable enemy units or NPCs.
  [CreateAssetMenu(menuName = "State/UnitState")]
  public class UnitState : ScriptableObject {
    public Vector3Int startingPosition;
    public UnitEncounterState encounterState;
    
    public List<UnitAbility> GetAbilities() {
      // TODO(P1): Add abilities from equipment etc.
      return new();
    }
  }
}