﻿using System;
using System.Linq;
using Events;
using StaticConfig.Builds;
using UnityEngine;

namespace HUD.ShipManagement.Crafting {
  public class CraftingMenu : MonoBehaviour {
    private Canvas _canvas;
    
    private void Awake() {
      _canvas = GetComponent<Canvas>();
      _canvas.enabled = false;
    }

    private void OnEnable() {
      Dispatch.ShipBuilder.InGameBuildClicked.RegisterListener(OnShipConstructionClicked);
      Dispatch.ShipBuilder.CloseCraftingMenu.RegisterListener(OnCloseCraftingMenu);
    }

    private void OnDisable() {
      Dispatch.ShipBuilder.InGameBuildClicked.UnregisterListener(OnShipConstructionClicked);
      Dispatch.ShipBuilder.CloseCraftingMenu.RegisterListener(OnCloseCraftingMenu);
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