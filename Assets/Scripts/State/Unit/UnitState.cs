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
    // TODO(P0): Replace with SO 
    public int maxHp;
    public UnitFaction faction;
    // TODO(P0): Replace with SO
    public int movementRange;
    public UnitEncounterState encounterState;
    
    public List<UnitAbility> GetAbilities() {
      // TODO(P1): Add abilities from equipment etc.
      return new();
    }

    private void OnEnable() {
      // TODO(P0): This is dangerous and probably will result in stacked up
      //    refs, but is necessary for now for the editor to behave.
      hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
  }

  public enum UnitFaction {
    PlayerParty,
    Enemy,
  }
}