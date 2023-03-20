using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CameraControl;
using Common;
using Common.Grid;
using Common.Loading;
using Controls;
using HUD.MainMenu;
using IngameDebugConsole;
using RuntimeVars;
using State;
using State.World;
using Unity.Burst.Intrinsics;
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
    [SerializeField] private GameObject borderPrefab;
    [SerializeField] private CommonEvents commonEvents;

    // Tiles 
    [SerializeField] private TileBase indicatorTile;
    [SerializeField] private TileBase openSeaTile;
    [SerializeField] private TileBase encounterTile;
    [SerializeField] private TileBase fogOfWarTile;
    [SerializeField] private TileBase outOfBoundsTile;
    [SerializeField] private TileBase defeatedEncounterTile;

    private Tilemap _overlayTilemap;
    private Tilemap _overworldTilemap;
    private AsyncOperation _loadShipBuilderSceneOperation;
    private MainMenuController _gameMenu;
    private CameraCursorMover _cameraMover;
    private GameControls _controls;
    private Camera _camera;
    private UiInteractionTracker _uiInteraction;
    private Grid _grid;

    private void Awake() {
      // TODO(P3): Refactor this class to pull some seperable logic out and prevent huge bloat.
      _grid = GameObject.Find("Grid").GetComponent<Grid>();
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
      
      commonEvents.dialogueStart.RegisterListener(OnDialogueStart);
      commonEvents.dialogueEnd.RegisterListener(OnDialogueEnd);
      DebugLogConsole.AddCommand("reveal", "Reveal all map tiles", RevealMap);
    }

    private void OnDisable() {
      _controls.Overworld.Disable();
      
      commonEvents.dialogueStart.UnregisterListener(OnDialogueStart);
      commonEvents.dialogueEnd.UnregisterListener(OnDialogueEnd);
      DebugLogConsole.RemoveCommand("reveal");
    }

    private void Start() {
      DisplayWorld();
      DisplayPlayerIndicator();
      RemoveFogOfWar();
      _gameMenu = MainMenuController.Get();
      _gameMenu.AddMenuItem(buildMenuItemLabel, OnConstructionMode);
      // Because the ship builder scene will be a common destination from here, pre-load it
      StartCoroutine(LoadShipBuilderScene());
      _cameraMover.Initialize(
          _overworldTilemap.GetCellCenterWorld(
              (Vector3Int)GameState.State.player.overworldGridPosition));
    }

    private void UpdateTile (WorldTile mapTile) {
      var tile = mapTile.state == TileState.Obscured ? fogOfWarTile : mapTile switch {
          var x when x.state == TileState.Cleared => defeatedEncounterTile,
          OpenSeaWorldTile => openSeaTile,
          EncounterWorldTile => encounterTile,
          OutOfBoundsWorldTile => outOfBoundsTile,
          _ => fogOfWarTile,
      };
      _overworldTilemap.SetTile(mapTile.coordinates.AsVector3Int(), tile);
    }

    private void RemoveFogOfWar() {
      Vector2Int currentPlayerPosition = GameState.State.player.overworldGridPosition;
      HexGridUtils.ForEachAdjacentTileInRange(currentPlayerPosition, GameState.State.player.visionRange, cell => {
        WorldTile mapTile = GameState.State.world.GetTile(cell);
        mapTile.Reveal();
        UpdateTile(mapTile);
      });
    }

    private void DisplayWorld() {
      foreach (var mapTile in GameState.State.world.tileContents.Values) {
        UpdateTile(mapTile);
      }
      DisplayBoundaries();
    }

    private void DisplayBoundaries() {
      foreach (var border in GameState.State.world.outpostBorders) {
        var path = border.GetWorldPath(_grid.GetCellCenterWorld);
        
        // TODO(P3): instantiate these into a container to not spam the root node.
        var borderLine = Instantiate(borderPrefab).GetComponent<LineRenderer>();
        borderLine.positionCount = path.Count;
        borderLine.SetPositions(path.ToArray());
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

      if (CanExecuteMapMove(gridCell)) {
        ExecuteMapMove(gridCell);
      }
    }
    
    // Exists for debugging only, for now
    public void OnRightClick(InputAction.CallbackContext context) {
      if (!context.performed || _uiInteraction.isPlayerHoveringUi) {
        return;
      }
      
      var worldPoint = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
      worldPoint.z = 0;
      var gridCell = _overworldTilemap.layoutGrid.WorldToCell(worldPoint);
      Debug.Log($"Clicked cell: {gridCell}");
    }

    private bool CanExecuteMapMove(Vector3Int gridCell) {
      if (!GameState.State.world.CanExecuteMove(gridCell)) {
        return false;
      }
      Vector2Int currentPlayerPosition = GameState.State.player.overworldGridPosition;
      bool isInBoundsX = (gridCell.x >= currentPlayerPosition.x - 1 && gridCell.x <= currentPlayerPosition.x + 1);
      bool isInBoundsY = (gridCell.y >= currentPlayerPosition.y - 1 && gridCell.y <= currentPlayerPosition.y + 1);
      return isInBoundsX && isInBoundsY;
    }
    
    private void ExecuteMapMove(Vector3Int gridCell) {
      MovePlayer(gridCell);
      RemoveFogOfWar();

      var destination = GameState.State.world.GetTile(gridCell.x, gridCell.y);
      destination.OnVisit();
    }

    private void MovePlayer(Vector3Int gridCell) {
      GameState.State.player.overworldGridPosition = (Vector2Int)gridCell;
      _cameraMover.MoveCursorDirectly(_overworldTilemap.GetCellCenterWorld(gridCell));
      _overlayTilemap.ClearAllTiles();
      _overlayTilemap.SetTile(gridCell, indicatorTile);
    }
    
    public void RevealMap() {
      foreach (var gameTile in GameState.State.world.tileContents.Values) {
        gameTile.Reveal();
        UpdateTile(gameTile);
      }
    }

    private void OnDialogueStart() {
      _controls.TurnBasedEncounter.Disable();
    }

    private void OnDialogueEnd() {
      _controls.TurnBasedEncounter.Enable();
    }
  }
}