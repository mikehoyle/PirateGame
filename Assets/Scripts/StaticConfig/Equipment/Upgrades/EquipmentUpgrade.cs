using System.Linq;
using Common;
using Optional;
using State;
using StaticConfig.Builds;
using StaticConfig.Units;
using Units.Abilities;
using Units.Abilities.Range;
using Units.Abilities.AOE;
using UnityEngine;
using State.Unit;

namespace StaticConfig.Equipment.Upgrades {
  [CreateAssetMenu(menuName = "Equipment/Upgrades/EquipmentUpgrade")]
  public class EquipmentUpgrade : ScriptableObject {
    // Metadata
    public string displayName;
    [Multiline] public string longDescription;
    public Sprite icon;
    
    // Prerequisites
    [SerializeField] private EquipmentUpgrade optionalPrerequisite;
    public LineItem[] cost;
    public UnitAbility[] optionalOnlyAppliesToAbilities;
    public bool onlyAppliesToAbilitiesFromSourceEquipment;
    
    // What it does
    public SerializableDictionary<Stat, int> flatStatModifiers;
    [SerializeReference, SerializeReferenceButton]
    public AbilityRange rangeOverride;
    [Multiline] public string aoeOverride;
    public bool allowMovementAfterUse;
    public int additionalUsesPerEncounter;
    public UnitAbility[] revokesAbilities;
    public UnitAbility[] addsAbilities;
    
    private Option<AreaOfEffect> _aoeOverride = Option.None<AreaOfEffect>();

    public int GetModifiedStat(
        PlayerUnitMetadata actor, UnitAbility ability, Stat stat, int currentValue) {
      if (AppliesTo(actor, ability) && flatStatModifiers.TryGetValue(stat, out var modifier)) {
        return currentValue + modifier;
      }
      return currentValue;
    }
    
    public int GetModifiedStatNoRestrictions(Stat stat, int currentValue) {
      if (flatStatModifiers.TryGetValue(stat, out var modifier)) {
        return currentValue + modifier;
      }
      return currentValue;
    }

    public Option<AbilityRange> GetRangeOverride(PlayerUnitMetadata playerUnitMetadata, UnitAbility ability) {
      if (AppliesTo(playerUnitMetadata, ability) && rangeOverride != null) {
        return Option.Some(rangeOverride);
      }

      return Option.None<AbilityRange>();
    }

    public Option<AreaOfEffect> GetAoeOverride(PlayerUnitMetadata playerUnitMetadata, UnitAbility ability) {
      if (AppliesTo(playerUnitMetadata, ability)) {
        return _aoeOverride;
      }

      return Option.None<AreaOfEffect>();
    }

    public bool GetAllowMovementAfterUse(PlayerUnitMetadata playerUnitMetadata, UnitAbility ability) {
      if (AppliesTo(playerUnitMetadata, ability) && allowMovementAfterUse) {
        return true;
      }
      return false;
    }

    public int GetAdditionalUsesPerEncounter(PlayerUnitMetadata playerUnitMetadata, UnitAbility ability) {
      if (!AppliesTo(playerUnitMetadata, ability)) {
        return 0;
      }
      return additionalUsesPerEncounter;
    }

    public Option<EquipmentUpgrade> GetPrerequisite() {
      if (optionalPrerequisite == null || optionalPrerequisite.IsNull()) {
        return Option.None<EquipmentUpgrade>();
      }
      return Option.Some(optionalPrerequisite);
    }

    public bool CanAfford() {
      return GameState.State.player.inventory.CanAfford(cost);
    }

    private bool AppliesTo(PlayerUnitMetadata actor, UnitAbility ability) {
      if (optionalOnlyAppliesToAbilities.Length > 0 && !optionalOnlyAppliesToAbilities.Contains(ability)) {
        return false;
      }
      
      if (onlyAppliesToAbilitiesFromSourceEquipment) {
        // Very hackily reverse-engineer which equipment provided the ability + upgrade.
        var abilityIsFromSourceEquipment = false;
        foreach (var equipmentInstance in actor.equipped.Values) {
          if (equipmentInstance.item.GetAllUpgradesOfAllTiers().Contains(this)
              && equipmentInstance.GetAbilitiesProvided().Contains(ability)) {
            abilityIsFromSourceEquipment = true;
          }
        }

        if (!abilityIsFromSourceEquipment) {
          return false;
        }
      }

      return true;
    }

    public int GetUpgradeTier() {
      if (!GetPrerequisite().TryGet(out var prereq)) {
        return 1;
      }

      return 1 + prereq.GetUpgradeTier();
    }

    private bool IsNull() {
      return displayName == "";
    }
    
    private void Awake() {
      UpdateAoeDefinition();
    }

    private void OnValidate() {
      UpdateAoeDefinition();
    }

    private void UpdateAoeDefinition() {
      if (string.IsNullOrEmpty(aoeOverride)) {
        return;
      }
      _aoeOverride = Option.Some(AoeParser.ParseAreaOfEffect(aoeOverride));
    }
  }
}