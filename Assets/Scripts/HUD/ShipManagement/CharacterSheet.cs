using System;
using PixelsoftGames.PixelUI;
using RuntimeVars.ShipBuilder.Events;
using Units;
using UnityEngine;

namespace HUD.ShipManagement {
  /// <summary>
  /// Controls character management UI.
  /// </summary>
  public class CharacterSheet : MonoBehaviour {
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;
    
    private UITabbedWindow _window;

    private void Awake() {
      _window = GetComponentInChildren<UITabbedWindow>();
      shipBuilderEvents.openCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      shipBuilderEvents.closeCharacterSheet.RegisterListener(OnCloseCharacterSheet);
    }

    private void Start() {
      // Deactivate in start so children get a chance to Awake
      gameObject.SetActive(false);
    }

    private void OnDestroy() {
      shipBuilderEvents.openCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      shipBuilderEvents.closeCharacterSheet.UnregisterListener(OnCloseCharacterSheet);
    }

    private void OnOpenCharacterSheet(UnitController unit) {
      gameObject.SetActive(true);
      _window.ActivateContentPane(0);
    }
    
    private void OnCloseCharacterSheet() {
      gameObject.SetActive(false);
    }
  }
}