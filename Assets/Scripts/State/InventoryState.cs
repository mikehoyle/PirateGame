using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using StaticConfig;

namespace State {
  [Serializable]
  public class InventoryState {
    // Maps item to quantity held, where the keys are RawResourceScriptableObject.ids
    private Dictionary<string, int> _items = new();

    public int GetQuantity(RawResourceScriptableObject item) {
      return _items.GetValueOrDefault(item.id);
    }

    public void AddQuantity(RawResourceScriptableObject item, int quantity) {
      SetQuantity(item, GetQuantity(item) + quantity);
    }
    
    public void ReduceQuantity(RawResourceScriptableObject item, int quantity) {
      AddQuantity(item, -quantity);
    }

    public void SetQuantity(RawResourceScriptableObject item, int quantity) {
      _items[item.id] = Math.Max(quantity, 0);
    }

    public bool CanAffordBuild([CanBeNull] ConstructableScriptableObject build) {
      if (build == null) {
        return false;
      }
      
      foreach (var lineItem in build.buildCost) {
        if (GetQuantity(lineItem.resource) < lineItem.cost) {
          return false;
        }
      }

      return true;
    }

    public void DeductBuildCost(ConstructableScriptableObject build) {
      foreach (var lineItem in build.buildCost) {
        ReduceQuantity(lineItem.resource, lineItem.cost);
      }
    }

    // It is very error-prone to use this directly, but it's convenient just for the
    // debug/prototype setup.
    public void DebugOnlySetQuantity(string itemId, int quantity) {
      _items[itemId] = quantity;
    }
  }
}