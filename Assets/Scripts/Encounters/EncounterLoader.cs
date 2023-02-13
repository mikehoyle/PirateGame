using System;
using Encounters.Grid;
using State;
using State.World;
using UnityEngine;

namespace Encounters {
  public class EncounterLoader : MonoBehaviour {
    private EncounterTile _encounter;
    private EncounterGenerator _encounterGenerator;
    private EncounterSetup _encounterSetup;
    private ShipPlacementManager _shipPlacementManager;

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
    }

    private void Start() {
      if (!_encounter.isInitialized) {
        _encounterGenerator.Generate(_encounter);
      }
      
      _encounterSetup.SetUpMap(_encounter);
      _shipPlacementManager.BeginShipPlacement(_encounter);
      enabled = false;
    }
  }
}