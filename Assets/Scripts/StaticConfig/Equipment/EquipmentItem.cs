using System.Collections.Generic;
using System.Text;
using Common.Animation;
using StaticConfig.Units;
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
    public SerializableDictionary<Stat, int> statBonuses; 

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