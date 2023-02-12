using System;
using Encounters.Grid;
using State;
using State.World;
using UnityEngine;

namespace Encounters {
  public class EncounterLoader : MonoBehaviour {
    [SerializeField] private GameObject encounterManagerPrefab;
    
    private EncounterTile _encounter;
    private EncounterGenerator _encounterGenerator;
    private EncounterSetup _encounterSetup;

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
    }

    private void Start() {
      if (!_encounter.isInitialized) {
        _encounterGenerator.Generate(_encounter);
      }
      
      _encounterSetup.SetUpMap(_encounter, GetShipPlacementOffset(GameState.State.player.ship));
      
      Instantiate(encounterManagerPrefab);
      Destroy(gameObject);
    }
    
    // For now, just put the ship next to the terrain in the y+ direction.
    private Vector3Int GetShipPlacementOffset(ShipState ship) {
      // TODO(P1): Let player place ship
      var terrainBounds = _encounter.terrain.GetBoundingRect();
      var shipBoundingRect = ship.components.GetBoundingRect();
      return new Vector3Int(
          terrainBounds.xMin - shipBoundingRect.xMin,
          (terrainBounds.yMax + 1) - shipBoundingRect.yMin,
          0
      );
    }
  }
}