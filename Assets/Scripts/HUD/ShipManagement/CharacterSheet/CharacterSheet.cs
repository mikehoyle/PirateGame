using PixelsoftGames.PixelUI;
using RuntimeVars.ShipBuilder.Events;
using Units;
using UnityEngine;

namespace HUD.ShipManagement.CharacterSheet {
  /// <summary>
  ///   Controls character management UI.
  /// </summary>
  public class CharacterSheet : MonoBehaviour {
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;
    private Canvas _canvas;

    private UITabbedWindow _window;

    private void Awake() {
      _window = GetComponentInChildren<UITabbedWindow>();
      _canvas = GetComponent<Canvas>();
      _canvas.enabled = false;
    }

    private void OnEnable() {
      shipBuilderEvents.openCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      shipBuilderEvents.closeCharacterSheet.RegisterListener(OnCloseCharacterSheet);
    }

    private void OnDisable() {
      shipBuilderEvents.openCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      shipBuilderEvents.closeCharacterSheet.UnregisterListener(OnCloseCharacterSheet);
    }

    private void OnOpenCharacterSheet(UnitController unit) {
      _canvas.enabled = true;
      _window.ActivateContentPane(0);
    }

    private void OnCloseCharacterSheet() {
      _canvas.enabled = false;
    }
  }
}