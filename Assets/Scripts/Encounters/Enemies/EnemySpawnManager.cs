using RuntimeVars.Encounters.Events;
using State.Unit;
using UnityEngine;

namespace Encounters.Enemies {
  public class EnemySpawnManager : MonoBehaviour {
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private GameObject spawnPointPrefab;
    [SerializeField] private float spawnIntervalSec = 0.5f;

    private int _unitsCurrentlySpawning;
    
    public void OnEnable() {
      encounterEvents.spawnEnemyRequest.RegisterListener(OnSpawnEnemyRequest);
      encounterEvents.enemyTurnPreEnd.RegisterListener(OnEnemyTurnPreEnd);
    }

    public void OnDisable() {
      encounterEvents.spawnEnemyRequest.UnregisterListener(OnSpawnEnemyRequest);
      encounterEvents.enemyTurnPreEnd.UnregisterListener(OnEnemyTurnPreEnd);
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
        encounterEvents.enemyTurnEnd.Raise();
      }
    }

    private void OnSpawnComplete() {
      Debug.Log("On Spawn complete callback");
      _unitsCurrentlySpawning--;
      if (_unitsCurrentlySpawning <= 0) {
        encounterEvents.enemyTurnEnd.Raise();
      }
    }
  }
}