using Common;
using UnityEngine;

namespace StaticConfig.Equipment {
  [CreateAssetMenu(menuName = "Equipment/EquipmentSlots")]
  public class EquipmentSlots : ScriptableObjectSingleton<EquipmentSlots> {
    public EquipmentSlot weaponSlot;
    public EquipmentSlot apparelSlot;
    public EquipmentSlot utilitySlot;
    
    protected override EquipmentSlots Self() {
      return this;
    }
  }
}