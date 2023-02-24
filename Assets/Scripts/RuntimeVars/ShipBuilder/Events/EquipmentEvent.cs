using Common.Events;
using StaticConfig.Equipment;
using UnityEngine;

namespace RuntimeVars.ShipBuilder.Events {
  [CreateAssetMenu(menuName = "Events/ShipBuilder/EquipmentEvent")]
  public class EquipmentEvent : ParameterizedGameEvent<EquipmentItemInstance> {}
}