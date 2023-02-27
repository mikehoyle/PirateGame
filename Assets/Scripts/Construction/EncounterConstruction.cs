using Common;
using Optional;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using Units;
using UnityEngine;

namespace Construction {
  public class EncounterConstruction : InGameConstruction {
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private CurrentSelection currentSelection;
    
    private void OnEnable() {
      encounterEvents.objectClicked.RegisterListener(OnObjectClicked);
    }

    private void OnDisable() {
      encounterEvents.objectClicked.UnregisterListener(OnObjectClicked);
    }

    private void OnObjectClicked(GameObject clickedObject) {
      if (clickedObject == gameObject) {
        TryProvideAbility();
      }
    }

    private void TryProvideAbility() {
      if (Metadata.providedAbility.ability == null
          || !currentSelection.selectedUnit.TryGet(out var actor)
          || actor is not UnitController playerActor
          || !Metadata.providedAbility.ability.CanAfford(actor)) {
        return;
      }
      
      var distance = GridUtils.DistanceBetween(Position, playerActor.Position);
      if (distance > Metadata.providedAbility.useRange) {
        Debug.Log("Actor too far to use provided ability");
        return;
      }

      playerActor.FaceTowards(Position);
      currentSelection.SelectAbility(playerActor, Metadata.providedAbility.ability, Position);
    }
  }
}