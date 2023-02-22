using RuntimeVars.ShipBuilder.Events;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement {
  public class CharacterStatsUi : MonoBehaviour {
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;
    private Text _text;

    private void Awake() {
      _text = GetComponentInChildren<Text>();
      shipBuilderEvents.openCharacterSheet.RegisterListener(OnOpenCharacterSheet);
    }

    private void OnDestroy() {
      shipBuilderEvents.openCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
    }

    private void OnOpenCharacterSheet(UnitController unit) {
      _text.text = $"{unit.Metadata.GetName()}\n\n";
      _text.text += $"Level: {unit.Metadata.currentLevel}";
    }
  }
}