using System;
using System.Linq;
using RuntimeVars.ShipBuilder.Events;
using StaticConfig.Builds;
using UnityEngine;

namespace HUD.ShipManagement.Crafting {
  public class CraftingMenu : MonoBehaviour {
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;
    private Canvas _canvas;
    
    private void Awake() {
      _canvas = GetComponent<Canvas>();
      _canvas.enabled = false;
    }

    private void OnEnable() {
      shipBuilderEvents.inGameBuildClicked.RegisterListener(OnShipConstructionClicked);
      shipBuilderEvents.closeCraftingMenu.RegisterListener(OnCloseCraftingMenu);
    }

    private void OnDisable() {
      shipBuilderEvents.inGameBuildClicked.UnregisterListener(OnShipConstructionClicked);
      shipBuilderEvents.closeCraftingMenu.RegisterListener(OnCloseCraftingMenu);
    }

    private void OnShipConstructionClicked(ConstructableObject constructableObject) {
      if (!constructableObject.providedCraftables.Any()) {
        return;
      }
      _canvas.enabled = true;
    }
    
    private void OnCloseCraftingMenu() {
      _canvas.enabled = false;
    }
  }
}