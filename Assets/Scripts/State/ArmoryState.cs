using System;
using System.Collections.Generic;
using StaticConfig.Equipment;

namespace State {
  [Serializable]
  public class ArmoryState {
    public List<EquipmentItemInstance> equipment;

    private ArmoryState() {
      equipment = new();
    }
  }
}