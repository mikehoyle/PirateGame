using Encounters;
using Events;
using Units;
using UnityEngine;

namespace HUD.Encounter.SelectionDetails {
  public class SelectionDetails : MonoBehaviour {

    private void Awake() {
      Dispatch.Encounters.UnitSelected.RegisterListener(OnUnitSelected);
    }

    private void OnDestroy() {
      Dispatch.Encounters.UnitSelected.UnregisterListener(OnUnitSelected);
    }

    private void Start() {
      gameObject.SetActive(false);
    }

    private void OnUnitSelected(EncounterActor unit) {
      gameObject.SetActive(unit != null);
    }
  }
}