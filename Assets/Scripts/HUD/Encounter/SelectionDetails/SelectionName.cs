using Encounters;
using RuntimeVars.Encounters.Events;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter.SelectionDetails {
  public class SelectionName : MonoBehaviour {
    [SerializeField] private EncounterEvents encounterEvents;
    
    private Text _text;

    private void Awake() {
      _text = GetComponent<Text>();
      encounterEvents.unitSelected.RegisterListener(OnUnitSelected);
    }

    private void OnDestroy() {
      encounterEvents.unitSelected.UnregisterListener(OnUnitSelected);
    }
    
    private void OnUnitSelected(EncounterActor unit) {
      if (unit != null) {
        _text.text = unit.EncounterState.metadata.GetName();
      }
    }
  }
}