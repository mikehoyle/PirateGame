using Events;
using State;
using StaticConfig.Builds;
using StaticConfig.Equipment;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement.Crafting {
  public class ItemCraftingOption : MonoBehaviour {
    [SerializeField] private GameObject lineItemPrefab;
    [SerializeField] private Color fulfilledRequirementColor;
    [SerializeField] private Color unfulfilledRequirementColor;

    private CraftingRecipe _recipe;
    private Button _button;

    private void Awake() {
      _button = GetComponent<Button>();
      _button.interactable = false;
    }

    private void OnEnable() {
      Dispatch.ShipBuilder.EquipmentCraftedEvent.RegisterListener(OnEquipmentCrafted);
      _button.onClick.AddListener(OnClickCraftButton);
    }
    
    private void OnDisable() {
      Dispatch.ShipBuilder.EquipmentCraftedEvent.UnregisterListener(OnEquipmentCrafted);
      _button.onClick.RemoveListener(OnClickCraftButton);
    }

    public void Initialize(CraftingRecipe recipe) {
      foreach (Transform child in transform) {
        Destroy(child.gameObject);
      }
      _recipe = recipe;
      
      var itemName = Instantiate(lineItemPrefab, transform).GetComponentInChildren<Text>();
      itemName.text = _recipe.result.DisplayDescription();
      
      var playerCanCurrentlyCraft = true;
      foreach (var lineItem in recipe.cost) {
        var text = Instantiate(lineItemPrefab, transform).GetComponentInChildren<Text>();
        var amountOfPlayerResource = GameState.State.player.inventory.GetQuantity(lineItem.resource);
        text.text = $"{lineItem.resource.displayName}: {amountOfPlayerResource}/{lineItem.cost}";
        if (amountOfPlayerResource < lineItem.cost) {
          text.color = unfulfilledRequirementColor;
          playerCanCurrentlyCraft = false;
        } else {
          text.color = fulfilledRequirementColor;
        }
      }

      foreach (var equipmentCost in recipe.equipmentCost) {
        var text = Instantiate(lineItemPrefab, transform).GetComponentInChildren<Text>();
        text.text = $"{equipmentCost.displayName}";
        if (GameState.State.player.armory.Has(equipmentCost)) {
          text.color = fulfilledRequirementColor;
        } else {
          text.color = unfulfilledRequirementColor;
          playerCanCurrentlyCraft = false;
        }
      }

      _button.interactable = playerCanCurrentlyCraft;
    }

    private void OnEquipmentCrafted(EquipmentItemInstance _) {
      // Re-initialize because we may no longer be able to afford the item.
      Initialize(_recipe);
    }

    private void OnClickCraftButton() {
      if (!_button.interactable) {
        return;
      }

      var playerInventory = GameState.State.player.inventory;
      foreach (var lineItem in _recipe.cost) {
        playerInventory.ReduceQuantity(lineItem.resource, lineItem.cost);
      }
      foreach (var equipmentItem in _recipe.equipmentCost) {
        GameState.State.player.armory.RemoveOne(equipmentItem);
      }
      var newInstance = new EquipmentItemInstance(_recipe.result);
      GameState.State.player.armory.equipment.Add(newInstance);
      Dispatch.ShipBuilder.EquipmentCraftedEvent.Raise(newInstance);
    }
  }
}