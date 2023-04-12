using Encounters;
using Events;
using PixelsoftGames.PixelUI;
using UnityEngine;

namespace HUD.ShipManagement.CharacterSheet {
  /// <summary>
  ///   Controls character management UI.
  /// </summary>
  public class CharacterSheet : MonoBehaviour {
    private Canvas _canvas;

    private UITabbedWindow _window;

    private void Awake() {
      _window = GetComponentInChildren<UITabbedWindow>();
      _canvas = GetComponent<Canvas>();
      _canvas.enabled = false;
    }

    // Currently defunct, probably due for deletion
    private void OnEnable() {
      //Dispatch.ShipBuilder.OpenCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      //Dispatch.ShipBuilder.CloseCharacterSheet.RegisterListener(OnCloseCharacterSheet);
    }

    private void OnDisable() {
      //Dispatch.ShipBuilder.OpenCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      //Dispatch.ShipBuilder.CloseCharacterSheet.UnregisterListener(OnCloseCharacterSheet);
    }

    private void OnOpenCharacterSheet(EncounterActor unit) {
      _canvas.enabled = true;
      _window.ActivateContentPane(0);
    }

    private void OnCloseCharacterSheet() {
      _canvas.enabled = false;
    }
  }
}