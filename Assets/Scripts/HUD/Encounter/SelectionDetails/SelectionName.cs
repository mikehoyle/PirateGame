using Encounters;
using Events;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter.SelectionDetails {
  public class SelectionName : MonoBehaviour {
    private Text _text;

    private void Awake() {
      _text = GetComponent<Text>();
      Dispatch.Encounters.UnitSelected.RegisterListener(OnUnitSelected);
    }

    private void OnDestroy() {
      Dispatch.Encounters.UnitSelected.UnregisterListener(OnUnitSelected);
    }
    
    private void OnUnitSelected(EncounterActor unit) {
      if (unit != null) {
        _text.text = unit.EncounterState.metadata.GetName();
      }
    }
  }
}