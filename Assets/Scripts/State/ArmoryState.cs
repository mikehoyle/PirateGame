using System.Collections.Generic;
using StaticConfig.Equipment;
using UnityEngine;

namespace State {
  [CreateAssetMenu(menuName = "State/ArmoryState")]
  public class ArmoryState : ScriptableObject {
    public List<EquipmentItemInstance> equipment;

    private ArmoryState() {
      equipment = new();
    }
  }
}