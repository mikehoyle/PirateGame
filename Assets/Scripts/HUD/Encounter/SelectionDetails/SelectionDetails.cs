using RuntimeVars.Encounters.Events;
using Units;
using UnityEngine;

namespace HUD.Encounter.SelectionDetails {
  public class SelectionDetails : MonoBehaviour {
    [SerializeField] private EncounterEvents encounterEvents;

    private void Awake() {
      encounterEvents.unitSelected.RegisterListener(OnUnitSelected);
    }

    private void OnDestroy() {
      encounterEvents.unitSelected.UnregisterListener(OnUnitSelected);
    }

    private void Start() {
      gameObject.SetActive(false);
    }

    private void OnUnitSelected(UnitController unit) {
      gameObject.SetActive(unit != null);
    }
  }
}