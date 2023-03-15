using System.Collections;
using Common.Grid;
using RuntimeVars.Encounters.Events;
using State.Unit;
using UnityEngine;

namespace Encounters.Enemies {
  // Represents a known area where an enemy will spawn.
  public class EnemySpawnPoint : MonoBehaviour, IPlacedOnGrid {
    [SerializeField] private EncounterEvents encounterEvents;
    
    public delegate void OnSpawnComplete();
    
    private UnitEncounterState _enemyToSpawn;
    private int _roundsUntilSpawn;
    private PolygonCollider2D _collider;
    
    
    public Vector3Int Position { get; private set; }
    public bool BlocksAllMovement => false;
    public bool ClaimsTile => true;

    private void Awake() {
      _collider = GetComponent<PolygonCollider2D>();
    }

    public void Init(UnitEncounterState enemyToSpawn, int roundsUntilSpawn) {
      _enemyToSpawn = enemyToSpawn;
      _roundsUntilSpawn = roundsUntilSpawn;
      Position = _enemyToSpawn.position;
      transform.position = GridUtils.CellAnchorWorldStatic(enemyToSpawn.position);
      ApplySize();
    }
    
    private void ApplySize() {
      _collider.offset = new Vector2(0, -GridUtils.CellHeightInWorldUnits);
      _collider.SetPath(0, GridUtils.GetFootprintPolygon(_enemyToSpawn.metadata.size));
    }

    private void OnEnable() {
      encounterEvents.enemyTurnStart.RegisterListener(OnEnemyTurnStart);
    }

    private void OnDisable() {
      encounterEvents.enemyTurnStart.UnregisterListener(OnEnemyTurnStart);
    }

    private void SpawnEnemy(OnSpawnComplete callback) {
      if (_enemyToSpawn.metadata is not EnemyUnitMetadata enemyMetadata) {
        Debug.LogWarning("Cannot spawn non-enemy");
        OnDropInComplete(callback);
        return;
      }
      var unitController = Instantiate(enemyMetadata.prefab).GetComponent<EnemyUnitController>();
      unitController.Init(_enemyToSpawn);
      unitController.DropIn(() => OnDropInComplete(callback));
    }

    public bool TrySpawn(float delaySecs, OnSpawnComplete callback) {
      if (_roundsUntilSpawn > 0) {
        return false;
      }
      StartCoroutine(ExecuteSpawn(delaySecs, callback));
      return true;
    }

    private IEnumerator ExecuteSpawn(float delaySecs, OnSpawnComplete callback) {
      yield return new WaitForSeconds(delaySecs);
      SpawnEnemy(callback);
    }

    private void OnEnemyTurnStart() {
      _roundsUntilSpawn -= 1;
    }

    private void OnDropInComplete(OnSpawnComplete callback) {
      callback();
      Destroy(gameObject);
    }
  }
}