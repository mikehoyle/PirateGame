using System.Collections.Generic;
using System.Text;
using Units.Abilities;
using UnityEngine;

namespace StaticConfig.Equipment {
  [CreateAssetMenu(menuName = "Equipment/Equipment")]
  public class EquipmentItem : ScriptableObject {
    public string displayName;
    public Sprite hudSprite;
    public EquipmentSlot applicableSlot;
    public List<UnitAbility> abilitiesProvided;

    public string DisplayDescription() {
      var result = new StringBuilder($"{displayName}\n");
      foreach (var ability in abilitiesProvided) {
        result.Append($" - {ability.displayString}: ({ability.CostString()})\n");
        if (ability.incurredEffect != null) {
          result.Append($"   - {ability.incurredEffect.DisplayString()}\n");
        }
      }
      return result.ToString();
    }
  }
}