using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using StaticConfig.Builds;
using StaticConfig.RawResources;
using UnityEngine;

namespace State {
  [Serializable]
  public class InventoryState {
    // Maps item to quantity held, where the keys are RawResourceScriptableObject.ids
    [SerializeField] private SerializableDictionary<RawResource, int> items;

    private InventoryState() {
      items = new();
    }

    public int GetQuantity(RawResource item) {
      items.TryGetValue(item, out var quantity);
      return quantity;
    }

    public void AddQuantity(RawResource item, int quantity) {
      SetQuantity(item, GetQuantity(item) + quantity);
    }
    
    public void ReduceQuantity(RawResource item, int quantity) {
      AddQuantity(item, -quantity);
    }

    public void SetQuantity(RawResource item, int quantity) {
      items[item] = Math.Max(quantity, 0);
    }

    public bool CanAffordBuild([CanBeNull] ConstructableObject build) {
      if (build == null) {
        return false;
      }

      return CanAfford(build.buildCost);
    }

    public bool CanAfford(IEnumerable<LineItem> lineItems) {
      foreach (var lineItem in lineItems) {
        if (GetQuantity(lineItem.resource) < lineItem.cost) {
          return false;
        }
      }

      return true;
    }

    public void DeductCost(IEnumerable<LineItem> lineItems) {
      foreach (var lineItem in lineItems) {
        ReduceQuantity(lineItem.resource, lineItem.cost);
      }
    }
  }
}