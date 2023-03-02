using Encounters;
using RuntimeVars.ShipBuilder.Events;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement.CharacterSheet {
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

    private void OnOpenCharacterSheet(EncounterActor unit) {
      if (unit is not PlayerUnitController playerUnit) {
        return;
      }
      _text.text = $"{playerUnit.Metadata.GetName()}\n\n";
      _text.text += $"Level: {playerUnit.Metadata.currentLevel}";
    }
  }
}