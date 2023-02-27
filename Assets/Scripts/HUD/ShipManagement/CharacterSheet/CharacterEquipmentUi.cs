using Common;
using RuntimeVars.Encounters;
using RuntimeVars.ShipBuilder.Events;
using State;
using StaticConfig.Equipment;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement.CharacterSheet {
  public class CharacterEquipmentUi : MonoBehaviour {
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private EquipmentSlot[] equipmentSlots;
    
    private Text _text;
    private bool _characterSheetActive;
    private UnitController _unit;

    private void Awake() {
      shipBuilderEvents.openCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      shipBuilderEvents.closeCharacterSheet.RegisterListener(OnCloseCharacterSheet);
      shipBuilderEvents.attemptEquipItem.RegisterListener(OnAttemptEquipItem);
      shipBuilderEvents.itemEquipped.RegisterListener(OnItemEquipped);
      _text = GetComponentInChildren<Text>();
    }

    private void OnDestroy() {
      shipBuilderEvents.openCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      shipBuilderEvents.closeCharacterSheet.UnregisterListener(OnCloseCharacterSheet);
      shipBuilderEvents.attemptEquipItem.UnregisterListener(OnAttemptEquipItem);
    }

    private void OnOpenCharacterSheet(UnitController unit) {
      _unit = unit;
      Refresh(unit);
    }
    
    private void OnItemEquipped(EquipmentItemInstance param) {
      Refresh(_unit);
    }

    private void Refresh(UnitController unit) {
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
          || unit is not UnitController playerUnit) {
        return;
      }
      
      Debug.Log($"Equipping {itemInstance.item.displayName}");

      var slot = itemInstance.item.applicableSlot;
      if (playerUnit.Metadata.equipped.TryGetValue(slot, out var currentSlotContents)) {
        GameState.State.player.armory.equipment.Add(currentSlotContents);
      }

      playerUnit.Metadata.equipped[slot] = itemInstance;
      GameState.State.player.armory.equipment.Remove(itemInstance);
      shipBuilderEvents.itemEquipped.Raise(itemInstance);
    }
  }
}