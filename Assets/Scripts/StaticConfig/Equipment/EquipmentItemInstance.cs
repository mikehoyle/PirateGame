using System;
using System.Collections.Generic;
using Common;
using Events;
using State;
using StaticConfig.Equipment.Upgrades;
using Units.Abilities;
using UnityEngine;

namespace StaticConfig.Equipment {
  /// <summary>
  /// Consider equipment instances separate than their configuration ScriptableObject, because
  /// a player might have more than one of the same, and in the future we may want to add
  /// values like durability.
  /// </summary>
  [Serializable]
  public class EquipmentItemInstance {
    public EquipmentItem item;
    public List<EquipmentUpgrade> appliedUpgrades;
    
    private EquipmentItemInstance() {
      appliedUpgrades = new();
    }
    
    public EquipmentItemInstance(EquipmentItem item) {
      this.item = item;
    }

    public List<UnitAbility> GetAbilitiesProvided() {
      var abilitiesProvided = new List<UnitAbility>(item.abilitiesProvided);
      var abilitiesRevoked = new List<UnitAbility>();

      foreach (var upgrade in appliedUpgrades) {
        abilitiesProvided.AddRange(upgrade.addsAbilities);
        abilitiesRevoked.AddRange(upgrade.revokesAbilities);
      }

      abilitiesProvided.RemoveAll(ability => abilitiesRevoked.Contains(ability));
      return abilitiesProvided;
    }

    public UpgradeState GetUpgradeState(EquipmentUpgrade upgrade) {
      if (appliedUpgrades.Contains(upgrade)) {
        return UpgradeState.Acquired;
      }

      if (!upgrade.GetPrerequisite().TryGet(out var prerequisite) || appliedUpgrades.Contains(prerequisite)) {
        // Available
        return upgrade.CanAfford() ? UpgradeState.Available : UpgradeState.AvailableButUnaffordable;
      }

      return UpgradeState.Locked;
    }

    public void AttemptPurchaseUpgrade(EquipmentUpgrade upgrade) {
      if (GetUpgradeState(upgrade) != UpgradeState.Available) {
        return;
      }
      
      Debug.Log($"Purchasing upgrade: {upgrade.displayName}");
      GameState.State.player.inventory.DeductCost(upgrade.cost);
      appliedUpgrades.Add(upgrade);
      Dispatch.ShipBuilder.EquipmentUpgradePurchased.Raise();
    }
  }
}