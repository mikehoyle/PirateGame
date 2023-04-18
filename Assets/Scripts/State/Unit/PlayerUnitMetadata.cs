using System.Collections.Generic;
using System.Linq;
using Common.Animation;
using StaticConfig.Equipment;
using StaticConfig.Equipment.Upgrades;
using StaticConfig.Units;
using Units;
using Units.Abilities;
using UnityEngine;

namespace State.Unit {
  [CreateAssetMenu(menuName = "State/UnitMetadata")]
  public class PlayerUnitMetadata : UnitMetadata {
    // TODO(P3): Make these configurable in an asset.
    private const int BaseHp = 10;
    private const int HpPerLevel = 1;
    private const int BaseMovement = 4;
    private const int MovementPerLevel = 1;

    [SerializeField] private UnitAbilitySet defaultAbilities;
    [SerializeField] private Stats allStats;
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
          unitAbilities.AddRange(equipment.GetAbilitiesProvided());
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

    public override ExhaustibleResourceTracker.GetResourceMax GetHpFormula() {
      return getStat => BaseHp + (HpPerLevel * getStat(allStats.constitution));
    }
    
    public override ExhaustibleResourceTracker.GetResourceMax GetMovementRangeFormula() {
      return getStat => BaseMovement + (MovementPerLevel * getStat(allStats.movement));
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

    public List<EquipmentUpgrade> GetAllUpgrades() {
      var result = new List<EquipmentUpgrade>();
      foreach (var equipment in equipped.Values) {
        result.AddRange(equipment.appliedUpgrades);
      }
      return result;
    } 

    public override int GetStat(Stat stat) {
      var statValue = base.GetStat(stat);
      foreach (var upgrade in GetAllUpgrades()) {
        statValue = upgrade.GetModifiedStatNoRestrictions(stat, statValue);
      }
      return statValue;
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