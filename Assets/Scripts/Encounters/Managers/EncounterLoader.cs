using System;
using CameraControl;
using Common.Grid;
using Encounters.ShipPlacement;
using State;
using State.World;
using UnityEngine;
using Zen.Hexagons;

namespace Encounters.Managers {
  public class EncounterLoader : MonoBehaviour {
    private EncounterWorldTile _encounter;
    private EncounterGenerator _encounterGenerator;
    private EncounterSetup _encounterSetup;
    private ShipPlacementManager _shipPlacementManager;
    private CameraCursorMover _cameraCursor;

    private void Awake() {
      MaybeLoadDebugEncounter();
      _encounter = GameState.State.world.GetActiveTile().DownCast<EncounterWorldTile>();
      if (_encounter == null) {
        throw new NotSupportedException(
            "Cannot load an encounter on a non-encounter tile." +
            $"Tile: {GameState.State.player.overworldGridPosition}," +
            $"Tile type: {GameState.State.world.GetActiveTile().GetType()}");
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
          GridUtils.CellCenterWorldStatic(new Vector3Int((int)center.x, (int)center.y, 0)));
      _cameraCursor.SetGridBounds(boundingRect);
    }
    
    
    private void MaybeLoadDebugEncounter() {
      if (Debug.isDebugBuild && GameState.State.world.GetActiveTile() == null) {
        var position = GameState.State.player.overworldGridPosition;
        var encounter = new EncounterWorldTile(HexOffsetCoordinates.From((Vector3Int)position)) {
            difficulty = 2,
        };
        GameState.State.world.UpdateTile(encounter);
      }
    }
  }
}