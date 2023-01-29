using System;
using System.Collections;
using HUD.MainMenu;
using State;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Overworld {
  public class OverworldGameManager : MonoBehaviour {
    [SerializeField] private string shipBuilderScene = "ShipBuilderScene";
    [SerializeField] private string buildMenuItemLabel = "Construction";
    [SerializeField] private TileBase indicatorTile;
    private Tilemap _overlayTilemap;
    private Tilemap _overworldTilemap;
    private AsyncOperation _loadShipBuilderSceneOperation;
    private MainMenuController _gameMenu;

    private void Awake() {
      _overworldTilemap = GameObject.Find("OverworldTilemap").GetComponent<Tilemap>();
      _overlayTilemap = GameObject.Find("OverlayTilemap").GetComponent<Tilemap>();
    }

    private void Start() {
      _gameMenu = MainMenuController.Get();
      _gameMenu.AddMenuItem(buildMenuItemLabel, OnConstructionMode);
      // Because the ship builder scene will be a common destination from here, pre-load it
      StartCoroutine(LoadShipBuilderScene());
    }

    private IEnumerator LoadShipBuilderScene() {
      _loadShipBuilderSceneOperation = SceneManager.LoadSceneAsync(shipBuilderScene, LoadSceneMode.Single);
      _loadShipBuilderSceneOperation.allowSceneActivation = false;
      while (!_loadShipBuilderSceneOperation.isDone) {
        yield return null;
      }
    }

    public void OnConstructionMode() {
      _loadShipBuilderSceneOperation.allowSceneActivation = true;
    }
  }
}