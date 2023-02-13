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
    [SerializeField] private GameObject inGameConstructionPrefab;
    
    private SceneTerrain _terrain;

    private void Awake() {
      _terrain = SceneTerrain.Get();
    }

    public void SetupShip(bool includeUnits = false) {
      SetupShip(Vector3Int.zero, includeUnits);
    }

    public void SetupShip(Vector3Int offset, bool includeUnits = false) {
      var playerState = GameState.State.player;
      foreach (var build in playerState.ship.components) {
        var position = build.Key + offset;
        AddBuild(position, build.Value);
      }

      if (includeUnits) {
        foreach (var unit in playerState.roster) {
          var unitController = Instantiate(unitPrefab).GetComponent<UnitController>();
          unitController.Init(unit, offset);
        }
      }
    }

    /// <summary>
    /// Ghost ship, i.e. a placeable immaterial ship that can be easily moved around
    /// </summary>
    public void SetupGhostShip(Transform parent) {
      foreach (var build in GameState.State.player.ship.components) {
        var position = build.Key;
        AddBuild(position, build.Value, parent, isGhost: true);
      }
    }

    public void AddBuild(
        Vector3Int position, ConstructableObject build, Transform parent = null, bool isGhost = false) {
      if (build.isFoundationTile && !isGhost) {
        _terrain.AddTerrain(position, build.inGameSprite);
        return;
      }

      Instantiate(inGameConstructionPrefab, parent)
          .GetComponent<InGameConstruction>()
          .Initialize(build, position);
    }
  }
}