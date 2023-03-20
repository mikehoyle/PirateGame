using System;
using System.Text;
using StaticConfig.RawResources;

namespace State.Encounter {
  [Serializable]
  public class CollectableInstance {
    public SerializableDictionary<RawResource, int> contents;
    public string name = "Collectable";
    public bool isPrimaryObjective;

    public void AddToPlayerInventory() {
      foreach (var resourceAmount in contents) {
        GameState.State.player.inventory.AddQuantity(resourceAmount.Key, resourceAmount.Value);
      }
    }

    public string DisplayString() {
      var result = new StringBuilder();
      foreach (var loot in contents) {
        result.Append($"+{loot.Value} {loot.Key.displayName}\n");
      }
      return result.ToString();
    }
  }
}