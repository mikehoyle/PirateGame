using RuntimeVars.ShipBuilder.Events;
using UnityEngine;

namespace Construction {
  public class ShipManagementConstruction : InGameConstruction {
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;

    private void OnEnable() {
      shipBuilderEvents.objectClicked.RegisterListener(OnObjectClicked);
    }

    private void OnDisable() {
      shipBuilderEvents.objectClicked.UnregisterListener(OnObjectClicked);
    }

    private void OnObjectClicked(GameObject clickedObject) {
      if (gameObject != clickedObject) {
        return;
      }
      
      // TODO(P0): Open context menu for construction.
    }
  }
}