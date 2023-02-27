using System;

namespace StaticConfig.Equipment {
  /// <summary>
  /// Consider equipment instances separate than their configuration ScriptableObject, because
  /// a player might have more than one of the same, and in the future we may want to add
  /// values like durability.
  /// </summary>
  [Serializable]
  public class EquipmentItemInstance {
    public EquipmentItem item;

    // provided for Unity serializer
    private EquipmentItemInstance() { }
    public EquipmentItemInstance(EquipmentItem item) {
      this.item = item;
    }
  }
}