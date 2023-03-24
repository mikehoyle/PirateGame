using Encounters;
using Events;
using State;
using StaticConfig.Builds;
using StaticConfig.Equipment;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement.Crafting {
  public class ArmoryPanel : MonoBehaviour {
    [SerializeField] private GameObject inventoryMenuItemPrefab;

    private void Awake() {
      Dispatch.ShipBuilder.OpenCraftingMenu.RegisterListener(RefreshDisplay);
      Dispatch.ShipBuilder.InGameBuildClicked.RegisterListener(OnShipConstructionClicked);
      Dispatch.ShipBuilder.EquipmentCraftedEvent.RegisterListener(OnEquipmentCrafted);
      Dispatch.ShipBuilder.OpenCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.ItemEquipped.RegisterListener(OnItemEquipped);
    }

    private void OnDestroy() {
      Dispatch.ShipBuilder.OpenCraftingMenu.UnregisterListener(RefreshDisplay);
      Dispatch.ShipBuilder.InGameBuildClicked.UnregisterListener(OnShipConstructionClicked);
      Dispatch.ShipBuilder.EquipmentCraftedEvent.UnregisterListener(OnEquipmentCrafted);
      Dispatch.ShipBuilder.OpenCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.ItemEquipped.UnregisterListener(OnItemEquipped);
    }

    private void OnShipConstructionClicked(ConstructableObject _) {
      RefreshDisplay();
    }

    private void OnEquipmentCrafted(EquipmentItemInstance _) {
      RefreshDisplay();
    }

    private void OnOpenCharacterSheet(EncounterActor _) {
      RefreshDisplay();
    }

    private void OnItemEquipped(EquipmentItemInstance _) {
      RefreshDisplay();
    }

    private void RefreshDisplay() {
      Clear();
      foreach (var equipmentInstance in GameState.State.player.armory.equipment) {
        Instantiate(inventoryMenuItemPrefab, transform).GetComponent<ArmoryItem>().Initialize(equipmentInstance);
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