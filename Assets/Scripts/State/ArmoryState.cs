using System;
using System.Collections.Generic;
using System.Linq;
using FunkyCode.Rendering.Light.Shadow;
using StaticConfig.Equipment;

namespace State {
  [Serializable]
  public class ArmoryState {
    public List<EquipmentItemInstance> equipment;

    private ArmoryState() {
      equipment = new();
    }

    public bool Has(EquipmentItem item) {
      foreach (var equipmentItemInstance in equipment) {
        if (equipmentItemInstance.item == item) {
          return true;
        }
      }
      return false;
    }

    public void RemoveOne(EquipmentItem item) {
      for (int i = 0; i < equipment.Count; i++) {
        if (equipment[i].item == item) {
          equipment.RemoveAt(i);
          return;
        }
      }
    }
  }
}