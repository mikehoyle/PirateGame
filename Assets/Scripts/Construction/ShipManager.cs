using System;
using CameraControl;
using Common;
using HUD.MainMenu;
using RuntimeVars.ShipBuilder.Events;
using State;
using Terrain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Construction {
  public class ShipManager : MonoBehaviour {
    [SerializeField] private string backToMapButtonLabel = "Back to Map";
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;

    private SceneTerrain _terrain;
    private ShipSetup _shipSetup;
    private MainMenuController _mainMenu;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _shipSetup = GetComponent<ShipSetup>();
    }

    private void Start() {
      _shipSetup.SetupShip(includeUnits: true);
      InitializeCamera();
      _mainMenu = MainMenuController.Get();
      _mainMenu.AddMenuItem(backToMapButtonLabel, OnBackToMap);
    }

    private void InitializeCamera() {
      var cameraMover = GetComponent<CameraCursorMover>();
      var minX = int.MaxValue;
      var maxX = int.MinValue;
      var minY = int.MaxValue;
      var maxY = int.MinValue;
      foreach (var tileCoord in GameState.State.player.ship.components.Keys) {
        minX = Math.Min(minX, tileCoord.x);
        maxX = Math.Max(maxX, tileCoord.x);
        minY = Math.Min(minY, tileCoord.y);
        maxY = Math.Max(maxY, tileCoord.y);
      }
      
      var visualMin = _terrain.Grid.CellToWorld(new Vector3Int(minX, minY, 0));
      // +1 to maxes because CellToWorld returns bottom corner of cell,
      // so top corner of cell = bottom corner of caddy-cornered cell.
      var visualMax = _terrain.Grid.CellToWorld(new Vector3Int(maxX + 1, maxY + 1, 0));
      cameraMover.Initialize(Vector3.Lerp(visualMin, visualMax, 0.5f));
    }

    private void OnBackToMap() {
      SceneManager.LoadScene(Scenes.Name.Overworld.SceneName());
    }
  }
}