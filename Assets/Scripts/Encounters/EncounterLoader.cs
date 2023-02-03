using System;
using State;
using State.World;
using UnityEngine;

namespace Encounters {
  public class EncounterLoader : MonoBehaviour {
    [SerializeField] private GameObject encounterManagerPrefab;
    
    private EncounterTile _encounter;
    private EncounterGenerator _encounterGenerator;
    private EncounterSetup _encounterSetup;
    private IsometricGrid _grid;

    private void Awake() {
      _encounter = GameState.State.World.GetActiveTile().DownCast<EncounterTile>();
      if (_encounter == null) {
        throw new NotSupportedException(
            "Cannot load an encounter on a non-encounter tile." +
            $"Tile: {GameState.State.Player.OverworldGridPosition}," +
            $"Tile type: {GameState.State.World.GetActiveTile().TileType}");
      }

      _encounterGenerator = GetComponent<EncounterGenerator>();
      _encounterSetup = GetComponent<EncounterSetup>();
      _grid = IsometricGrid.Get();
    }

    private void Start() {
      if (!_encounter.IsInitialized) {
        _encounterGenerator.Generate(_encounter);
      }
      
      _encounterSetup.SetUpMap(_encounter, GetShipPlacementOffset(GameState.State.Player.Ship));
      
      Instantiate(encounterManagerPrefab);
      Destroy(gameObject);
    }
    
    // For now, just put the ship next to the terrain in the y+ direction.
    private Vector3Int GetShipPlacementOffset(ShipState ship) {
      // TODO(P1): Let player place ship
      var terrainBounds = _encounter.Terrain.GetBoundingRect();
      var shipBoundingRect = ship.Components.GetBoundingRect();
      return new Vector3Int(
          terrainBounds.xMin - shipBoundingRect.xMin,
          (terrainBounds.yMax + 1) - shipBoundingRect.yMin,
          0
      );
    }
  }
}