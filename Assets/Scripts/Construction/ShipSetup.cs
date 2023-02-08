using Encounters;
using Encounters.Grid;
using JetBrains.Annotations;
using Pathfinding;
using State;
using StaticConfig;
using StaticConfig.Builds;
using Units;
using UnityEngine;

namespace Construction {
  /// <summary>
  /// Sets up the player's ship to appear in a scene with an isometric grid.
  /// </summary>
  public class ShipSetup : MonoBehaviour {
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;
    [SerializeField] private GameObject unitPrefab;
    
    private IsometricGrid _grid;
    [CanBeNull] private EncounterTerrain _terrain;

    private void Awake() {
      _grid = IsometricGrid.Get();
      _terrain = EncounterTerrain.Get();
    }

    public void SetupShip(bool includeUnits = false) {
      SetupShip(Vector3Int.zero);
    }

    public void SetupShip(Vector3Int offset, bool includeUnits = false) {
      var playerState = GameState.State.player;
      foreach (var build in playerState.ship.components) {
        var position = build.Key + offset;
        _grid.Tilemap.SetTile(position, build.Value.inGameTile);
        _terrain?.MarkCellTraversable(position);
      }

      if (includeUnits) {
        foreach (var unit in playerState.roster) {
          var unitController = Instantiate(unitPrefab).GetComponent<UnitController>();
          unitController.Init(unit, offset);
        }
      }
    }
  }
}