using System;
using StaticConfig.RawResources;

namespace State.Encounter {
  [Serializable]
  public class CollectableInstance {
    public SerializableDictionary<RawResource, int> contents;

    public void AddToPlayerInventory() {
      foreach (var resourceAmount in contents) {
        GameState.State.player.inventory.AddQuantity(resourceAmount.Key, resourceAmount.Value);
      }
    }
  }
}