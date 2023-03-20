using Events;
using StaticConfig.Builds;
using UnityEngine;

namespace HUD.ShipManagement.Crafting {
  public class CraftingPanel : MonoBehaviour {
    [SerializeField] private GameObject craftingMenuItemPrefab;

    private void OnEnable() {
      Dispatch.ShipBuilder.InGameBuildClicked.RegisterListener(OnShipConstructionClicked);
    }

    private void OnDisable() {
      Dispatch.ShipBuilder.InGameBuildClicked.UnregisterListener(OnShipConstructionClicked);
    }

    private void OnShipConstructionClicked(ConstructableObject constructableObject) {
      Clear();
      foreach (var recipe in constructableObject.providedCraftables) {
        Instantiate(craftingMenuItemPrefab, transform).GetComponent<ItemCraftingOption>()
            .Initialize(recipe);
      }
    }

    private void Clear() {
      foreach (Transform child in transform) {
        Destroy(child.gameObject);
      }
    }
  }
}