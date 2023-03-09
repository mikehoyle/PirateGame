using System.Collections.Generic;
using System.Linq;
using Common.Animation;
using StaticConfig.Equipment;
using StaticConfig.Units;
using Units;
using Units.Abilities;
using UnityEngine;

namespace State.Unit {
  [CreateAssetMenu(menuName = "State/UnitMetadata")]
  public class PlayerUnitMetadata : UnitMetadata {
    // TODO(P3): Make these configurable in an asset.
    private const int BaseHp = 4;
    private const int HpPerLevel = 9;
    private const int BaseMovement = 3;
    private const int MovementPerLevel = 1;

    [SerializeField] private UnitAbilitySet defaultAbilities;
    [SerializeField] private Stat constitutionStat;
    [SerializeField] private Stat movementStat;
    [SerializeField] private UnitAbility debugGodSmite;

    public string firstName;
    public string lastName;
    public int currentLevel = 1;
    public int xp;
    public Vector3Int startingPosition;
    public SerializableDictionary<EquipmentSlot, EquipmentItemInstance> equipped;

    public override List<UnitAbility> GetAbilities() {
      List<UnitAbility> unitAbilities = defaultAbilities.abilities.ToList();
      foreach (var equipment in equipped.Values) {
        if (equipment.item is not null) {
          unitAbilities.AddRange(equipment.item.abilitiesProvided);
        }
      }

      if (Debug.isDebugBuild && debugGodSmite != null) {
        unitAbilities.Add(debugGodSmite);
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

    public void GrantXp(int xpGained) {
      xp += xpGained;
      if (xp >= ExperienceCalculations.GetLevelRequirement(currentLevel + 1)) {
        // TODO(P2): Add some fanfare for leveling up.
        currentLevel += 1;
      }
    }
  }
}