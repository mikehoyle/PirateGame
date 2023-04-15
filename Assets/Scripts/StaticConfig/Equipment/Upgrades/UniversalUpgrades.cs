using System.Collections.Generic;
using Common;
using UnityEngine;

namespace StaticConfig.Equipment.Upgrades {
  [CreateAssetMenu(menuName = "Equipment/Upgrades/UniversalUpgrades")]
  public class UniversalUpgrades : ScriptableObjectSingleton<UniversalUpgrades> {
    public List<EquipmentUpgrade> weaponUpgrades;
    public List<EquipmentUpgrade> apparelUpgrades;
    public List<EquipmentUpgrade> utilityUpgrades;

    public List<EquipmentUpgrade> GetUpgradesForSlot(EquipmentSlot slot) {
      if (slot == EquipmentSlots.Instance.weaponSlot) {
        return weaponUpgrades;
      } else if (slot == EquipmentSlots.Instance.apparelSlot) {
        return apparelUpgrades;
      } else if (slot == EquipmentSlots.Instance.utilitySlot) {
        return utilityUpgrades;
      }
      return new();
    }

    protected override UniversalUpgrades Self() {
      return this;
    }
  }
}