using System.Collections;
using Common;
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
      var x = spawnPointPrefab?.transform;
    }

    private IEnumerator OnEnemyTurnPreEnd() {
      var currentSpawnDelay = 0f;
      _unitsCurrentlySpawning = 0;
      foreach (Transform spawnPoint in transform) {
        if (spawnPoint.GetComponent<EnemySpawnPoint>().TrySpawn(currentSpawnDelay).TryGet(out var coroutine)) {
          yield return coroutine;
          currentSpawnDelay += spawnIntervalSec;
        }
      }
    }
  }
}