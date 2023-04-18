using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Animation;
using Optional;
using StaticConfig.Equipment.Upgrades;
using Units.Abilities;
using UnityEngine;

namespace StaticConfig.Equipment {
  [CreateAssetMenu(menuName = "Equipment/Equipment")]
  public class EquipmentItem : ScriptableObject {
    public string displayName;
    public string description;
    public Sprite hudSprite;
    public DirectionalAnimatedSprite optionalEquippedSprite;
    public EquipmentSlot applicableSlot;
    public List<UnitAbility> abilitiesProvided;
    public List<EquipmentUpgrade> availableUpgrades;

    public IEnumerable<EquipmentUpgrade> GetAvailableUpgrades() {
      return availableUpgrades.Concat(UniversalUpgrades.Instance.GetUpgradesForSlot(applicableSlot));
    }

    public IEnumerable<EquipmentUpgrade> GetAllUpgradesOfAllTiers() {
      return GetAvailableUpgrades().SelectMany(GetUpgradeAndAllPrereqs);
    }

    private List<EquipmentUpgrade> GetUpgradeAndAllPrereqs(EquipmentUpgrade topTier) {
      var result = new List<EquipmentUpgrade>();

      var current = Option.Some(topTier);
      while (current.TryGet(out var currentPrereq)) {
        result.Add(currentPrereq);
        current = currentPrereq.GetPrerequisite();
      }

      return result;
    }

    public string DisplayDescription() {
      var result = new StringBuilder($"{displayName}\n");
      if (!string.IsNullOrEmpty(description)) {
        result.Append($" - {description}");
      }
      foreach (var ability in abilitiesProvided) {
        result.Append($" - {ability.displayString}: ({ability.CostString()})\n");
        result.Append($"   - {ability.descriptionShort}");
      }
      return result.ToString();
    }
  }
}