using System;
using System.Collections.Generic;

namespace State {
  [Serializable]
  public class InventoryState {
    public enum Item {
      None,
      Lumber,
    }
    
    public Dictionary<Item, int> Items = new();
  }
}