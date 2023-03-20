using Common;
using Encounters;
using Events;
using RuntimeVars.Encounters;
using State;
using StaticConfig.Equipment;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement.CharacterSheet {
  public class CharacterEquipmentUi : MonoBehaviour {
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private EquipmentSlot[] equipmentSlots;
    
    private Text _text;
    private bool _characterSheetActive;
    private PlayerUnitController _unit;

    private void Awake() {
      Dispatch.ShipBuilder.OpenCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.CloseCharacterSheet.RegisterListener(OnCloseCharacterSheet);
      Dispatch.ShipBuilder.AttemptEquipItem.RegisterListener(OnAttemptEquipItem);
      Dispatch.ShipBuilder.ItemEquipped.RegisterListener(OnItemEquipped);
      _text = GetComponentInChildren<Text>();
    }

    private void OnDestroy() {
      Dispatch.ShipBuilder.OpenCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.CloseCharacterSheet.UnregisterListener(OnCloseCharacterSheet);
      Dispatch.ShipBuilder.AttemptEquipItem.UnregisterListener(OnAttemptEquipItem);
    }

    private void OnOpenCharacterSheet(EncounterActor unit) {
      if (unit is not PlayerUnitController playerUnit) {
        return;
      }
      _unit = playerUnit;
      Refresh(playerUnit);
    }
    
    private void OnItemEquipped(EquipmentItemInstance param) {
      Refresh(_unit);
    }

    private void Refresh(PlayerUnitController unit) {
      _characterSheetActive = true;
      _text.text = $"{unit.Metadata.GetName()}\n\n";
      foreach (var equipmentSlot in equipmentSlots) {
        if (unit.Metadata.equipped.TryGetValue(equipmentSlot, out var equipment)) {
          _text.text += $"{equipmentSlot.displayName}: {equipment.item.DisplayDescription()}\n";
        } else {
          _text.text += $"{equipmentSlot.displayName}: None\n";
        }
      }
    }

    private void OnCloseCharacterSheet() {
      _characterSheetActive = false;
    }

    private void OnAttemptEquipItem(EquipmentItemInstance itemInstance) {
      if (!_characterSheetActive
          || !currentSelection.selectedUnit.TryGet(out var unit)
          || unit is not PlayerUnitController playerUnit) {
        return;
      }
      
      Debug.Log($"Equipping {itemInstance.item.displayName}");

      var slot = itemInstance.item.applicableSlot;
      if (playerUnit.Metadata.equipped.TryGetValue(slot, out var currentSlotContents)) {
        GameState.State.player.armory.equipment.Add(currentSlotContents);
      }

      playerUnit.Metadata.equipped[slot] = itemInstance;
      GameState.State.player.armory.equipment.Remove(itemInstance);
      Dispatch.ShipBuilder.ItemEquipped.Raise(itemInstance);
    }
  }
}