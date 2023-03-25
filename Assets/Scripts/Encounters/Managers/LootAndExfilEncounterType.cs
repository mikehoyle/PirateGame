using System.Collections;
using Encounters.Obstacles;
using Events;
using RuntimeVars;
using State;
using State.Encounter;
using State.Unit;
using Terrain;
using UnityEngine;
using UnityEngine.UI;

namespace Encounters.Managers {
  public class LootAndExfilEncounterType : EncounterType {
    [SerializeField] private string getKeyHintText;
    [SerializeField] private string exfilHintText;
    [SerializeField] private GameObject spiritKeyPrefab;
    [SerializeField] private EnemyUnitTypeCollection spawnableEnemies;
    [SerializeField] private TileCollection shipTiles;
    [SerializeField] private IntegerVar currentRound;
    [SerializeField] private float perRoundDifficultyScaling = 0.1f;
    [SerializeField] private float baseDrPercentForPerRoundSpawns = 0.5f;
    
    private Text _actionHint;
    private GameObject _exfilButton;
    private SceneTerrain _terrain;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _actionHint = transform.Find("ActionHint").GetComponent<Text>();
      _actionHint.gameObject.SetActive(false);
      _exfilButton = transform.Find("ExfilButton").gameObject;
      _exfilButton.SetActive(false);
    }

    protected override void OnEnable() {
      base.OnEnable();
      Dispatch.Encounters.EncounterStart.RegisterListener(OnEncounterStart);
      Dispatch.Encounters.ItemCollected.RegisterListener(OnItemCollected);
      Dispatch.Encounters.PlayerTurnStart.RegisterListener(OnPlayerTurnStart);
      Dispatch.Encounters.SpawnCollectable.RegisterListener(OnSpawnCollectable);
    }

    protected override void OnDisable() {
      base.OnDisable();
      Dispatch.Encounters.EncounterStart.UnregisterListener(OnEncounterStart);
      Dispatch.Encounters.ItemCollected.UnregisterListener(OnItemCollected);
      Dispatch.Encounters.PlayerTurnStart.UnregisterListener(OnPlayerTurnStart);
      Dispatch.Encounters.SpawnCollectable.UnregisterListener(OnSpawnCollectable);
    }

    private void OnEncounterStart() {
      _actionHint.gameObject.SetActive(true);
      _actionHint.text = getKeyHintText;
      StartCoroutine(FlashObjective());
    }

    private IEnumerator FlashObjective() {
      for (int i = 0; i < 6; i++) {
        yield return new WaitForSeconds(0.5f);
        _actionHint.gameObject.SetActive(!_actionHint.gameObject.activeInHierarchy);
      }
    }

    private void OnItemCollected(CollectableInstance item) {
      if (!item.isPrimaryObjective) {
        return;
      }

      _actionHint.text = exfilHintText;
      _exfilButton.SetActive(true);
      StartCoroutine(FlashObjective());
    }
    
    private void OnPlayerTurnStart() {
      var spawnDifficulty = (GameState.State.world.GetActiveTile().difficulty * baseDrPercentForPerRoundSpawns)
          + (currentRound.Value * perRoundDifficultyScaling);
      foreach (var enemy in spawnableEnemies.RandomEnemySpawnsForDifficulty(spawnDifficulty)) {
        var destination = _terrain.GetRandomAvailableTile(enemy.size, tile => !shipTiles.Contains(tile));
        var enemyEncounter = enemy.NewEncounter(destination);
        Dispatch.Encounters.SpawnEnemyRequest.Raise(enemyEncounter, 1);
      }
    }

    private void OnSpawnCollectable(Vector3Int position, CollectableInstance collectableInstance) {
      if (!collectableInstance.isPrimaryObjective) {
        return;
      }

      // TODO(P1): Animate and all that.
      Instantiate(spiritKeyPrefab).GetComponent<EncounterCollectable>().Initialize(collectableInstance, position);
    }
  }
}