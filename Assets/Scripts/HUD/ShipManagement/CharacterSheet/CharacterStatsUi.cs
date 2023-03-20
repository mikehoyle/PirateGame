using Encounters;
using Events;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement.CharacterSheet {
  public class CharacterStatsUi : MonoBehaviour {
    private Text _text;

    private void Awake() {
      _text = GetComponentInChildren<Text>();
      Dispatch.ShipBuilder.OpenCharacterSheet.RegisterListener(OnOpenCharacterSheet);
    }

    private void OnDestroy() {
      Dispatch.ShipBuilder.OpenCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
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