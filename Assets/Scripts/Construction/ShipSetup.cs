using State;
using StaticConfig.Builds;
using Terrain;
using Units;
using UnityEngine;

namespace Construction {
  /// <summary>
  /// Sets up the player's ship to appear in a scene with an isometric grid.
  /// </summary>
  public class ShipSetup : MonoBehaviour {
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;
    [SerializeField] private GameObject unitPrefab;
    
    private SceneTerrain _terrain;

    private void Awake() {
      _terrain = SceneTerrain.Get();
    }

    public void SetupShip(bool includeUnits = false) {
      SetupShip(Vector3Int.zero);
    }

    public void SetupShip(Vector3Int offset, bool includeUnits = false) {
      var playerState = GameState.State.player;
      foreach (var build in playerState.ship.components) {
        var position = build.Key + offset;
        _terrain.AddTerrain(position, build.Value.inGameSprite);
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