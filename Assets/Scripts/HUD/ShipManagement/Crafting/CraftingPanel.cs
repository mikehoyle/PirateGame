using Events;
using StaticConfig.Builds;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement.Crafting {
  public class CraftingPanel : MonoBehaviour {
    [SerializeField] private GameObject craftingMenuItemPrefab;
    [SerializeField] private CraftsCollection allShipCraftables;

    private void OnEnable() {
      Dispatch.ShipBuilder.OpenCraftingMenu.RegisterListener(OnOpenCraftingMenu);
      Dispatch.ShipBuilder.InGameBuildClicked.RegisterListener(OnShipConstructionClicked);
    }

    private void OnDisable() {
      Dispatch.ShipBuilder.OpenCraftingMenu.UnregisterListener(OnOpenCraftingMenu);
      Dispatch.ShipBuilder.InGameBuildClicked.UnregisterListener(OnShipConstructionClicked);
    }

    private void OnOpenCraftingMenu() {
      Clear();
      foreach (var recipe in allShipCraftables.craftingRecipes) {
        Instantiate(craftingMenuItemPrefab, transform).GetComponent<ItemCraftingOption>()
            .Initialize(recipe);
      }
      GetComponentInParent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
    }

    private void OnShipConstructionClicked(ConstructableObject constructableObject) {
      Clear();
      foreach (var recipe in constructableObject.providedCraftables) {
        Instantiate(craftingMenuItemPrefab, transform).GetComponent<ItemCraftingOption>()
            .Initialize(recipe);
      }
      GetComponentInParent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
    }

    private void Clear() {
      foreach (Transform child in transform) {
        Destroy(child.gameObject);
      }
    }
  }
}