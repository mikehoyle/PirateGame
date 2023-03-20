using Encounters;
using Events;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  public class EnterConstructionButton : MonoBehaviour {
    [SerializeField] private string enterConstructionText = "Enter Construction";
    [SerializeField] private string exitConstructionText = "Exit Construction";
    
    private Text _text;
    private bool _currentlyInConstructionMode;

    private void Awake() {
      _text = GetComponentInChildren<Text>();
      _currentlyInConstructionMode = false;
      Dispatch.ShipBuilder.OpenCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.CloseCharacterSheet.RegisterListener(OnCloseCharacterSheet);
      gameObject.SetActive(true);
    }

    private void OnDestroy() {
      Dispatch.ShipBuilder.OpenCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.CloseCharacterSheet.UnregisterListener(OnCloseCharacterSheet);
    }

    private void OnEnable() {
      Dispatch.ShipBuilder.EnterConstructionMode.RegisterListener(OnEnterConstruction);
      Dispatch.ShipBuilder.ExitConstructionMode.RegisterListener(OnExitConstruction);
    }

    private void OnDisable() {
      Dispatch.ShipBuilder.EnterConstructionMode.UnregisterListener(OnEnterConstruction);
      Dispatch.ShipBuilder.ExitConstructionMode.UnregisterListener(OnExitConstruction);
    }

    private void OnEnterConstruction() {
      _text.text = exitConstructionText;
      _currentlyInConstructionMode = true;
    }

    private void OnExitConstruction() {
      _text.text = enterConstructionText;
      _currentlyInConstructionMode = false;
    }

    private void OnOpenCharacterSheet(EncounterActor _) {
      gameObject.SetActive(false);
    }
    
    private void OnCloseCharacterSheet() {
      gameObject.SetActive(true);
    }

    // Button event handler
    public void OnClick() {
      if (_currentlyInConstructionMode) {
        Dispatch.ShipBuilder.ExitConstructionMode.Raise();
      } else {
        Dispatch.ShipBuilder.EnterConstructionMode.Raise();
      }
    }
  }
}