using Encounters.Enemies;
using Encounters.Obstacles;
using RuntimeVars.Encounters.Events;
using State.Unit;
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
    [SerializeField] private EncounterEvents encounterEvents;
    
    private SceneTerrain _terrain;
    private EncounterTile _encounter;

    private void Awake() {
      _terrain = SceneTerrain.Get();
    }

    private void OnEnable() {
      encounterEvents.encounterReadyToStart.RegisterListener(OnEncounterReady);
      encounterEvents.encounterStart.RegisterListener(OnEncounterStart);
    }

    private void OnDisable() {
      encounterEvents.encounterReadyToStart.UnregisterListener(OnEncounterReady);
      encounterEvents.encounterStart.UnregisterListener(OnEncounterStart);
    }

    public void SetUpMap(EncounterTile encounter) {
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
      encounterEvents.encounterStart.Raise();
      enabled = false;
    }

    private void SetUpEnemyUnits() {
      foreach (var enemy in _encounter.enemies) {
        encounterEvents.spawnEnemyRequest.Raise(enemy, 1);
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