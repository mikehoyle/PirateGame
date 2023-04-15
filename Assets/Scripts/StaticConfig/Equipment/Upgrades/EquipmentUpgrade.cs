using System.Linq;
using Optional;
using State;
using StaticConfig.Builds;
using StaticConfig.Units;
using Units.Abilities;
using Units.Abilities.Range;
using UnityEngine;

namespace StaticConfig.Equipment.Upgrades {
  [CreateAssetMenu(menuName = "Equipment/Upgrades/EquipmentUpgrade")]
  public class EquipmentUpgrade : ScriptableObject {
    public string displayName;
    public Sprite icon;
    [SerializeField] private EquipmentUpgrade optionalPrerequisite;
    public LineItem[] cost;
    public UnitAbility[] optionalOnlyAppliesToAbilities;
    
    // What it does
    public SerializableDictionary<Stat, int> flatStatModifiers;
    [SerializeReference, SerializeReferenceButton]
    public AbilityRange rangeOverride;
    public UnitAbility[] revokesAbilities;
    public UnitAbility[] addsAbilities;
    
    

    public int GetModifiedStat(UnitAbility ability, Stat stat, int currentValue) {
      if (AppliesTo(ability) && flatStatModifiers.TryGetValue(stat, out var modifier)) {
        return currentValue + modifier;
      }
      return currentValue;
    }

    public Option<AbilityRange> GetRangeOverride(UnitAbility ability) {
      if (AppliesTo(ability) && rangeOverride != null) {
        return Option.Some(rangeOverride);
      }

      return Option.None<AbilityRange>();
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

    private bool AppliesTo(UnitAbility ability) {
      return optionalOnlyAppliesToAbilities.Length == 0 || optionalOnlyAppliesToAbilities.Contains(ability);
    }

    public bool IsNull() {
      return displayName == "";
    }
  }
}