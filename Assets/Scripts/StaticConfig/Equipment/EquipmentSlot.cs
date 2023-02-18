using Common;
using UnityEngine;

namespace StaticConfig.Equipment {
  [CreateAssetMenu(menuName = "Equipment/EquipmentSlot")]
  public class EquipmentSlot : EnumScriptableObject {
    public string displayName;
  }
}