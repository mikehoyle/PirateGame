using System;
using RuntimeVars.ShipBuilder.Events;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  public class EnterConstructionButton : MonoBehaviour {
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;
    [SerializeField] private string enterConstructionText = "Enter Construction";
    [SerializeField] private string exitConstructionText = "Exit Construction";
    
    private Text _text;
    private bool _currentlyInConstructionMode;

    private void Awake() {
      _text = GetComponentInChildren<Text>();
      _currentlyInConstructionMode = false;
      shipBuilderEvents.openCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      shipBuilderEvents.closeCharacterSheet.RegisterListener(OnCloseCharacterSheet);
      gameObject.SetActive(true);
    }

    private void OnDestroy() {
      shipBuilderEvents.openCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      shipBuilderEvents.closeCharacterSheet.UnregisterListener(OnCloseCharacterSheet);
    }

    private void OnEnable() {
      shipBuilderEvents.enterConstructionMode.RegisterListener(OnEnterConstruction);
      shipBuilderEvents.exitConstructionMode.RegisterListener(OnExitConstruction);
    }

    private void OnDisable() {
      shipBuilderEvents.enterConstructionMode.UnregisterListener(OnEnterConstruction);
      shipBuilderEvents.exitConstructionMode.UnregisterListener(OnExitConstruction);
    }

    private void OnEnterConstruction() {
      _text.text = exitConstructionText;
      _currentlyInConstructionMode = true;
    }

    private void OnExitConstruction() {
      _text.text = enterConstructionText;
      _currentlyInConstructionMode = false;
    }

    private void OnOpenCharacterSheet(UnitController _) {
      gameObject.SetActive(false);
    }
    
    private void OnCloseCharacterSheet() {
      gameObject.SetActive(true);
    }

    // Button event handler
    public void OnClick() {
      if (_currentlyInConstructionMode) {
        shipBuilderEvents.exitConstructionMode.Raise();
      } else {
        shipBuilderEvents.enterConstructionMode.Raise();
      }
    }
  }
}