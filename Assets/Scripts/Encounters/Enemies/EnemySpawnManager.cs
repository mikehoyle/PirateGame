using Events;
using State.Unit;
using UnityEngine;

namespace Encounters.Enemies {
  public class EnemySpawnManager : MonoBehaviour {
    [SerializeField] private GameObject spawnPointPrefab;
    [SerializeField] private float spawnIntervalSec = 0.5f;

    private int _unitsCurrentlySpawning;
    
    public void OnEnable() {
      Dispatch.Encounters.SpawnEnemyRequest.RegisterListener(OnSpawnEnemyRequest);
      Dispatch.Encounters.EnemyTurnPreEnd.RegisterListener(OnEnemyTurnPreEnd);
    }

    public void OnDisable() {
      Dispatch.Encounters.SpawnEnemyRequest.UnregisterListener(OnSpawnEnemyRequest);
      Dispatch.Encounters.EnemyTurnPreEnd.UnregisterListener(OnEnemyTurnPreEnd);
    }
    
    
    private void OnSpawnEnemyRequest(UnitEncounterState enemy, int roundsUntilSpawn) {
      Instantiate(spawnPointPrefab, transform).GetComponent<EnemySpawnPoint>()
          .Init(enemy, roundsUntilSpawn);
    }

    private void OnEnemyTurnPreEnd() {
      var currentSpawnDelay = 0f;
      _unitsCurrentlySpawning = 0;
      foreach (Transform spawnPoint in transform) {
        if (spawnPoint.GetComponent<EnemySpawnPoint>().TrySpawn(currentSpawnDelay, OnSpawnComplete)) {
          _unitsCurrentlySpawning++;
          currentSpawnDelay += spawnIntervalSec;
        }
      }
      if (_unitsCurrentlySpawning == 0) {
        Dispatch.Encounters.EnemyTurnEnd.Raise();
      }
    }

    private void OnSpawnComplete() {
      _unitsCurrentlySpawning--;
      if (_unitsCurrentlySpawning <= 0) {
        Dispatch.Encounters.EnemyTurnEnd.Raise();
      }
    }
  }
}