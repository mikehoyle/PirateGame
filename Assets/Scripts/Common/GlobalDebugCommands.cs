using Events;
using IngameDebugConsole;
using State;
using StaticConfig.RawResources;
using UnityEngine;

namespace Common {
  public class GlobalDebugCommands : MonoBehaviour {
    private void Start() {
      DebugLogConsole.AddCommand<int, string>("give", "", GiveResources);
      DebugLogManager.Instance.OnLogWindowShown = OnLogWindowShown;
      DebugLogManager.Instance.OnLogWindowHidden = OnLogWindowHidden;
    }

    private static void GiveResources(int amount, string resourceName) {
      resourceName = resourceName.ToLower();

      var inventory = GameState.State.player.inventory;
      if (resourceName == "all") {
        foreach (var soulType in SoulTypes.Instance.All()) {
          inventory.AddQuantity(soulType, amount);
        }
      }

      if (resourceName == "violent" || resourceName == "v" || resourceName == SoulTypes.Instance.violent.id) {
        inventory.AddQuantity(SoulTypes.Instance.violent, amount);
      }
      if (resourceName == "diligent" || resourceName == "d" || resourceName == SoulTypes.Instance.diligent.id) {
        inventory.AddQuantity(SoulTypes.Instance.diligent, amount);
      }
      if (resourceName == "treacherous" || resourceName == "t" || resourceName == SoulTypes.Instance.treacherous.id) {
        inventory.AddQuantity(SoulTypes.Instance.treacherous, amount);
      }
      if (resourceName == "kind" || resourceName == "k" || resourceName == SoulTypes.Instance.kind.id) {
        inventory.AddQuantity(SoulTypes.Instance.kind, amount);
      }
    }

    private void OnLogWindowShown() {
      GameInput.AddInputBlocker(this);
      //Dispatch.Common.DebugLogWindowOpened.Raise();
    }

    private void OnLogWindowHidden() {
      GameInput.RemoveInputBlocker(this);
      //Dispatch.Common.DebugLogWindowClosed.Raise();
    }
  }
}