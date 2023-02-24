using System;
using System.Collections.Generic;
using StaticConfig.Equipment;

namespace StaticConfig.Builds {
  [Serializable]
  public class CraftingRecipe {
    public EquipmentItem result;
    public List<LineItem> cost;
  }
}