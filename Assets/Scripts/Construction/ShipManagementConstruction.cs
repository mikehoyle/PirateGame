using Events;
using UnityEngine;

namespace Construction {
  public class ShipManagementConstruction : InGameConstruction {

    private void OnEnable() {
      Dispatch.Common.ObjectClicked.RegisterListener(OnObjectClicked);
    }

    private void OnDisable() {
      Dispatch.Common.ObjectClicked.UnregisterListener(OnObjectClicked);
    }

    private void OnObjectClicked(GameObject clickedObject) {
      if (gameObject != clickedObject) {
        return;
      }

      Dispatch.ShipBuilder.InGameBuildClicked.Raise(Metadata);
    }
  }
}