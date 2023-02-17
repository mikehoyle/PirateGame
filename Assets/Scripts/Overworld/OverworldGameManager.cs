﻿using System.Collections;
using CameraControl;
using Common;
using Controls;
using HUD.MainMenu;
using State;
using State.World;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Overworld {
  /// <summary>
  /// TODO(P2): This scene is pretty static. Consider optimizing load times by background
  /// loading the content of every single surrounding tile, so entering any individual one
  /// would be instant.
  /// </summary>
  public class OverworldGameManager : MonoBehaviour, GameControls.IOverworldActions {
    [SerializeField] private string buildMenuItemLabel = "Construction";
    
    // Tiles 
    [SerializeField] private TileBase indicatorTile;
    [SerializeField] private TileBase openSeaTile;
    [SerializeField] private TileBase encounterTile;
    [SerializeField] private TileBase fogOfWarTile;
    
    private Tilemap _overlayTilemap;
    private Tilemap _overworldTilemap;
    private AsyncOperation _loadShipBuilderSceneOperation;
    private MainMenuController _gameMenu;
    private CameraCursorMover _cameraMover;
    private GameControls _controls;
    private Camera _camera;
    private UiInteractionTracker _uiInteraction;

    private void Awake() {
      _overworldTilemap = GameObject.Find("OverworldTilemap").GetComponent<Tilemap>();
      _overlayTilemap = GameObject.Find("OverlayTilemap").GetComponent<Tilemap>();
      _cameraMover = GetComponent<CameraCursorMover>();
      _uiInteraction = GetComponent<UiInteractionTracker>();
      _camera = Camera.main;
    }
    
    private void OnEnable() {
      if (_controls == null) {
        _controls ??= new GameControls();
        _controls.Overworld.SetCallbacks(this);
      }

      _controls.Overworld.Enable();
    }

    private void OnDisable() {
      _controls.Overworld.Disable();
    }

    private void Start() {
      DisplayWorld();
      DisplayPlayerIndicator();
      _gameMenu = MainMenuController.Get();
      _gameMenu.AddMenuItem(buildMenuItemLabel, OnConstructionMode);
      // Because the ship builder scene will be a common destination from here, pre-load it
      StartCoroutine(LoadShipBuilderScene());
      _cameraMover.Initialize(
          _overworldTilemap.GetCellCenterWorld(
              (Vector3Int)GameState.State.player.overworldGridPosition));
    }

    private void DisplayWorld() {
      foreach (var mapTile in GameState.State.world.tileContents.Values) {
        var tile = mapTile.TileType switch {
            WorldTile.Type.OpenSea => openSeaTile,
            WorldTile.Type.Encounter => encounterTile,
            _ => fogOfWarTile,
        };
        _overworldTilemap.SetTile(
            new Vector3Int(mapTile.coordinates.X, mapTile.coordinates.Y, 0), tile);
      }
    }
    
    private void DisplayPlayerIndicator() {
      _overlayTilemap.ClearAllTiles();
      _overlayTilemap.SetTile(
          (Vector3Int)GameState.State.player.overworldGridPosition, indicatorTile);
    }

    private IEnumerator LoadShipBuilderScene() {
      _loadShipBuilderSceneOperation = SceneManager.LoadSceneAsync(
          Scenes.Name.ShipBuilder.SceneName(), LoadSceneMode.Single);
      _loadShipBuilderSceneOperation.allowSceneActivation = false;
      while (!_loadShipBuilderSceneOperation.isDone) {
        yield return null;
      }
    }

    public void OnConstructionMode() {
      _loadShipBuilderSceneOperation.allowSceneActivation = true;
    }

    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed || _uiInteraction.isPlayerHoveringUi) {
        return;
      }
      
      var worldPoint = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
      worldPoint.z = 0;
      var gridCell = _overworldTilemap.layoutGrid.WorldToCell(worldPoint);

      ExecuteMapMove(gridCell);
    }
    
    private void ExecuteMapMove(Vector3Int gridCell) {
      // TODO(P1): Make a lot of changes and refinements here, like expending resources to travel,
      //     and doing any sort of adjacency checks.
      MovePlayer(gridCell);
      var destination = GameState.State.world.GetTile(gridCell.x, gridCell.y);
      switch (destination.TileType) {
        case WorldTile.Type.Encounter:
          // TODO(P0): Actually load up the encounter from known encounter params
          SceneManager.LoadScene(Scenes.Name.Encounter.SceneName());
          break;
      }
    }
    private void MovePlayer(Vector3Int gridCell) {
      GameState.State.player.overworldGridPosition = (Vector2Int)gridCell;
      _cameraMover.MoveCursorDirectly(_overworldTilemap.GetCellCenterWorld(gridCell));
      _overlayTilemap.ClearAllTiles();
      _overlayTilemap.SetTile(gridCell, indicatorTile);
    }
  }
}