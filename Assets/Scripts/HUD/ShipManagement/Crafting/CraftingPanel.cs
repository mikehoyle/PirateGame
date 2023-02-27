using RuntimeVars.ShipBuilder.Events;
using StaticConfig.Builds;
using UnityEngine;

namespace HUD.ShipManagement.Crafting {
  public class CraftingPanel : MonoBehaviour {
    [SerializeField] private GameObject craftingMenuItemPrefab;
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;

    private void OnEnable() {
      shipBuilderEvents.inGameBuildClicked.RegisterListener(OnShipConstructionClicked);
    }

    private void OnDisable() {
      shipBuilderEvents.inGameBuildClicked.UnregisterListener(OnShipConstructionClicked);
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