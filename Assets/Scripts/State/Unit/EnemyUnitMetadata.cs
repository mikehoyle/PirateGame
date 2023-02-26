using System.Collections.Generic;
using System.Linq;
using StaticConfig.Sprites;
using Units.Abilities;
using UnityEngine;

namespace State.Unit {
  [CreateAssetMenu(menuName = "State/EnemyUnitMetadata")]
  public class EnemyUnitMetadata : UnitMetadata {
    public string displayName;
    public int startingHp;
    public int movementRange;
    public DirectionalAnimatedSprite sprite;
    public UnitAbility[] abilities;

    public override List<UnitAbility> GetAbilities() {
      return abilities.ToList();
    }

    public override string GetName() {
      return displayName;
    }

    public override int GetStartingHp() {
      return startingHp;
    }
    
    public override int GetMovementRange() {
      return movementRange;
    }
    
    public UnitEncounterState NewEncounter(Vector3Int position) {
      return new UnitEncounterState {
          metadata = this,
          resources = GetEncounterTrackers(),
          position = position,
          facingDirection = FacingDirection.SouthEast,
          faction = UnitFaction.Enemy,
      };
    }
  }
}