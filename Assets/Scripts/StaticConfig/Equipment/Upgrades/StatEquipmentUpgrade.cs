using StaticConfig.Units;
using UnityEngine;

namespace StaticConfig.Equipment.Upgrades {
  [CreateAssetMenu(menuName = "Equipment/Upgrades/StatUpgrade")]
  public class StatEquipmentUpgrade : EquipmentUpgrade {
    public SerializableDictionary<Stat, int> statModifiers;
  }
}