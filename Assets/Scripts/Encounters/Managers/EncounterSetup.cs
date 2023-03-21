using Encounters.Enemies;
using Encounters.Obstacles;
using Events;
using State.World;
using Terrain;
using UnityEngine;

namespace Encounters.Managers {
  /// <summary>
  /// Sets up the scene-tree objects associated with the encounter.
  /// </summary>
  public class EncounterSetup : MonoBehaviour {
    [SerializeField] private Sprite landSprite;
    [SerializeField] private GameObject collectablePrefab;
    [SerializeField] private GameObject obstaclePrefab;
    
    private SceneTerrain _terrain;
    private EncounterWorldTile _encounter;

    private void Awake() {
      _terrain = SceneTerrain.Get();
    }

    private void OnEnable() {
      Dispatch.Encounters.ShipPlaced.RegisterListener(OnEncounterReady);
      Dispatch.Encounters.EncounterStart.RegisterListener(OnEncounterStart);
    }

    private void OnDisable() {
      Dispatch.Encounters.ShipPlaced.UnregisterListener(OnEncounterReady);
      Dispatch.Encounters.EncounterStart.UnregisterListener(OnEncounterStart);
    }

    public void SetUpMap(EncounterWorldTile encounter) {
      _encounter = encounter;
      foreach (var tile in encounter.terrain) {
        // For now, ignoring tile type because there's only one. In the future, probably use a scriptable
        // object to define tile for different types.
        _terrain.AddTerrain(tile.Key, landSprite);
      }
    }

    private void OnEncounterReady() {
      SetUpEnemyUnits();
      SetUpObstacles();
      SetUpCollectables();
      Dispatch.Encounters.EncounterSetUp.Raise();
      enabled = false;
    }

    private void SetUpEnemyUnits() {
      foreach (var enemy in _encounter.enemies) {
        var unitController = Instantiate(enemy.metadata.prefab).GetComponent<EnemyUnitController>();
        unitController.Init(enemy);
      }
    }

    private void SetUpObstacles() {
      foreach (var obstacle in _encounter.obstacles) {
        Instantiate(obstaclePrefab).GetComponent<EncounterObstacle>()
            .Initialize(obstacle.Value, obstacle.Key);
      }
    }

    private void SetUpCollectables() {
      foreach (var collectable in _encounter.collectables) {
        Instantiate(collectablePrefab).GetComponent<EncounterCollectable>()
            .Initialize(collectable.Value, collectable.Key);
      }
    }

    private void OnEncounterStart() {
      enabled = false;
    }
  }
}