using Common.Animation;
using Encounters;
using Events;
using UnityEngine;

namespace Units {
  public class ShipManagementUnit : MonoBehaviour {
    private CompositeDirectionalAnimator _sprite;
    
    private void Awake() {
      _sprite = GetComponent<CompositeDirectionalAnimator>();
    }

    private void OnEnable() {
      Dispatch.ShipBuilder.OpenCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.CloseCharacterSheet.RegisterListener(OnCloseCharacterSheet);
    }

    private void OnDisable() {
      Dispatch.ShipBuilder.OpenCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.CloseCharacterSheet.UnregisterListener(OnCloseCharacterSheet);
    }
    
    
    private void OnOpenCharacterSheet(EncounterActor unit) {
      if (gameObject != unit.gameObject) {
        _sprite.SetVisible(false);
      }
    }
    
    private void OnCloseCharacterSheet() {
      _sprite.SetVisible(true);
    }
  }
}