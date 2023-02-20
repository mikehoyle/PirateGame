using RuntimeVars.ShipBuilder.Events;
using StaticConfig.Equipment;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement {
  public class CharacterEquipmentUi : MonoBehaviour {
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;
    [SerializeField] private EquipmentSlot[] equipmentSlots;
    private Text _text;

    private void Awake() {
      shipBuilderEvents.openCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      _text = GetComponentInChildren<Text>();
    }

    private void OnDestroy() {
      shipBuilderEvents.openCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
    }

    private void OnOpenCharacterSheet(UnitController unit) {
      _text.text = $"{unit.Metadata.GetName()}\n\n";
      foreach (var equipmentSlot in equipmentSlots) {
        if (unit.Metadata.equipped.TryGetValue(equipmentSlot, out var equipment)) {
          _text.text += $"{equipmentSlot.displayName}: {equipment.displayName}\n";
          foreach (var ability in equipment.abilitiesProvided) {
            _text.text += $" - {ability.displayString}: ({ability.CostString()})\n";
            if (ability.incurredEffect != null) {
              _text.text += $"   - {ability.incurredEffect.DisplayString()}\n";
            }
          }
        } else {
          _text.text += $"{equipmentSlot.displayName}: None\n";
        }
      }
    }
  }
}