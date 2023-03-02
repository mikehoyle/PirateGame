using System;
using Encounters;
using RuntimeVars.ShipBuilder.Events;
using State;
using StaticConfig.Builds;
using StaticConfig.Equipment;
using Units;
using UnityEngine;

namespace HUD.ShipManagement.Crafting {
  public class ArmoryPanel : MonoBehaviour {
    [SerializeField] private GameObject inventoryMenuItemPrefab;
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;

    private void Awake() {
      shipBuilderEvents.inGameBuildClicked.RegisterListener(OnShipConstructionClicked);
      shipBuilderEvents.equipmentCraftedEvent.RegisterListener(OnEquipmentCrafted);
      shipBuilderEvents.openCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      shipBuilderEvents.itemEquipped.RegisterListener(OnItemEquipped);
    }

    private void OnDestroy() {
      shipBuilderEvents.inGameBuildClicked.UnregisterListener(OnShipConstructionClicked);
      shipBuilderEvents.equipmentCraftedEvent.UnregisterListener(OnEquipmentCrafted);
      shipBuilderEvents.openCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      shipBuilderEvents.itemEquipped.UnregisterListener(OnItemEquipped);
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
    }

    private void Clear() {
      foreach (Transform child in transform) {
        Destroy(child.gameObject);
      }
    }
  }
}