using System.Collections.Generic;
using Common;
using Common.Grid;
using Events;
using HUD.Encounter.HoverDetails;
using Optional;
using RuntimeVars.Encounters;
using Units;
using UnityEngine;

namespace Construction {
  public class EncounterConstruction : InGameConstruction, IDisplayDetailsProvider {
    [SerializeField] private CurrentSelection currentSelection;
    
    private void OnEnable() {
      Dispatch.Common.ObjectClicked.RegisterListener(OnObjectClicked);
    }

    private void OnDisable() {
      Dispatch.Common.ObjectClicked.UnregisterListener(OnObjectClicked);
    }

    private void OnObjectClicked(GameObject clickedObject) {
      if (clickedObject == gameObject) {
        TryProvideAbility();
      }
    }

    private void TryProvideAbility() {
      if (Metadata.providedAbility.ability == null
          || !currentSelection.TryGetUnit<PlayerUnitController>(out var playerActor)
          || !Metadata.providedAbility.ability.CanAfford(playerActor)) {
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
    
    public DisplayDetails GetDisplayDetails() {
      var additionalDetails = new List<string>();
      if (Metadata.providedAbility != null) {
        additionalDetails.Add(Metadata.providedAbility.DisplayString());
      }
      return new DisplayDetails {
          Name = Metadata.buildDisplayName,
          AdditionalDetails = additionalDetails,
      };
    }
  }
}