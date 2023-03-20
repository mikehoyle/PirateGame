using Encounters;
using Events;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  public class ManageCharacterButton : MonoBehaviour {
    private Button _button;
    private PlayerUnitController _parent;

    private void Awake() {
      _button = GetComponent<Button>();
      _button.onClick.AddListener(OnClick);
      _parent = GetComponentInParent<PlayerUnitController>();
      Dispatch.Encounters.UnitSelected.RegisterListener(OnUnitSelected);
      gameObject.SetActive(false);
    }

    private void OnDestroy() {
      Dispatch.Encounters.UnitSelected.UnregisterListener(OnUnitSelected);
    }

    private void OnClick() {
      Dispatch.ShipBuilder.OpenCharacterSheet.Raise(_parent);
    }
    
    private void OnUnitSelected(EncounterActor unit) {
      if (unit == null || unit != _parent) {
        gameObject.SetActive(false);
        return;
      }
      
      gameObject.SetActive(true);
    }
  }
}