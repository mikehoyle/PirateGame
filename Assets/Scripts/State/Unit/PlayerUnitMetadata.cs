﻿using System.Collections.Generic;
using System.Linq;
using StaticConfig.Equipment;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace State.Unit {
  // TODO(P1): Create reasonable separation for far-less-capable enemy units or NPCs.
  [CreateAssetMenu(menuName = "State/UnitState")]
  public class PlayerUnitMetadata : UnitMetadata {
    // TODO(P3): Make these configurable in an asset.
    private const int BaseHp = 10;
    private const int HpPerLevel = 10;
    private const int BaseMovement = 4;
    private const int MovementPerLevel = 1;

    [SerializeField] private UnitAbilitySet defaultAbilities;
    [SerializeField] private Stat constitutionStat;
    [SerializeField] private Stat movementStat;

    public string firstName;
    public string lastName;

    public Vector3Int startingPosition;
    public SerializableDictionary<EquipmentSlot, Equipment> equipped;

    public override List<UnitAbility> GetAbilities() {
      List<UnitAbility> unitAbilities = defaultAbilities.abilities.ToList();
      foreach (var equipment in equipped.Values) {
        unitAbilities.AddRange(equipment.abilitiesProvided);
      }
      return unitAbilities;
    }

    public override string GetName() {
      return firstName + " " + lastName;
    }

    public override int GetStartingHp() {
      return BaseHp + (HpPerLevel * GetStat(constitutionStat));
    }
    
    public override int GetMovementRange() {
      return BaseMovement + (MovementPerLevel * GetStat(movementStat));
    }

    public UnitEncounterState NewEncounter(Vector3Int shipOffset) {
      return new UnitEncounterState {
          metadata = this,
          resources = GetEncounterTrackers(),
          position = startingPosition + shipOffset,
          facingDirection = FacingDirection.SouthEast,
          faction = UnitFaction.PlayerParty,
      };
    }
  }
}