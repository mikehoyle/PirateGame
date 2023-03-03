using System;
using CameraControl;
using Encounters.ShipPlacement;
using State;
using State.World;
using Terrain;
using UnityEngine;

namespace Encounters.Managers {
  public class EncounterLoader : MonoBehaviour {
    private EncounterTile _encounter;
    private EncounterGenerator _encounterGenerator;
    private EncounterSetup _encounterSetup;
    private ShipPlacementManager _shipPlacementManager;
    private CameraCursorMover _cameraCursor;

    private void Awake() {
      _encounter = GameState.State.world.GetActiveTile().DownCast<EncounterTile>();
      if (_encounter == null) {
        throw new NotSupportedException(
            "Cannot load an encounter on a non-encounter tile." +
            $"Tile: {GameState.State.player.overworldGridPosition}," +
            $"Tile type: {GameState.State.world.GetActiveTile().TileType}");
      }

      _encounterGenerator = GetComponent<EncounterGenerator>();
      _encounterSetup = GetComponent<EncounterSetup>();
      _shipPlacementManager = GetComponent<ShipPlacementManager>();
      _cameraCursor = GetComponent<CameraCursorMover>();
    }

    private void Start() {
      if (!_encounter.isInitialized) {
        _encounterGenerator.Generate(_encounter);
      }
      
      _encounterSetup.SetUpMap(_encounter);
      SetupCamera();
      _shipPlacementManager.BeginShipPlacement(_encounter);
      enabled = false;
    }

    private void SetupCamera() {
      var boundingRect = _encounter.terrain.GetBoundingRect();
      var center = boundingRect.center;
      _cameraCursor.Initialize(
          SceneTerrain.CellCenterWorldStatic(new Vector3Int((int)center.x, (int)center.y, 0)));
      _cameraCursor.SetGridBounds(boundingRect);
    }
  }
}