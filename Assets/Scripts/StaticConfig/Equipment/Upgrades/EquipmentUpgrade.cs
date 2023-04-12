using StaticConfig.RawResources;
using Units.Abilities;
using UnityEngine;

namespace StaticConfig.Equipment.Upgrades {
  [CreateAssetMenu(menuName = "Equipment/Upgrades/EquipmentUpgrade")]
  public class EquipmentUpgrade : ScriptableObject {
    public string displayName;
    public EquipmentUpgrade optionalPrerequisite;
    public SerializableDictionary<RawResource, int> cost;
    public UnitAbility[] optionalOnlyAppliesToAbilities;
  }
}