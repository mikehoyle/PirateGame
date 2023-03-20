using Encounters;
using Events;
using RuntimeVars;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class ExfilButton : MonoBehaviour {
    [SerializeField] private UnitCollection playerUnitsInEncounter;
    [SerializeField] private TileCollection shipTiles;
    
    private Button _button;
    private Text _text;

    private void Awake() {
      _button = GetComponent<Button>();
      _text = GetComponentInChildren<Text>();
      _button.interactable = false;
      _button.onClick.AddListener(OnClick);
    }

    private void Update() {
      var unitsOnShip = 0;
      foreach (var unit in playerUnitsInEncounter) {
        if (shipTiles.Contains(unit.Position)) {
          unitsOnShip++;
        }
      }

      if (unitsOnShip < playerUnitsInEncounter.Count) {
        _text.text = $"{unitsOnShip}/{playerUnitsInEncounter.Count} On Board";
        _button.interactable = false;
      } else {
        _text.text = "Embark!";
        _button.interactable = true;
      }
    }

    private void OnClick() {
      Dispatch.Encounters.EncounterEnd.Raise(EncounterOutcome.PlayerVictory);
    }
  }
}