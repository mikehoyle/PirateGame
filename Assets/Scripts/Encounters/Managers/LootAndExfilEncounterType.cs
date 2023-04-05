using System;
using System.Collections;
using Encounters.Effects;
using Encounters.Obstacles;
using Events;
using RuntimeVars;
using State;
using State.Encounter;
using State.Unit;
using State.World;
using StaticConfig.Encounters;
using StaticConfig.RawResources;
using Terrain;
using UnityEngine;
using UnityEngine.UI;

namespace Encounters.Managers {
  public class LootAndExfilEncounterType : EncounterType {
    [SerializeField] private string getKeyHintText;
    [SerializeField] private string exfilHintText;
    [SerializeField] private GameObject spiritKeyPrefab;
    [SerializeField] private GameObject secondaryPickupPrefab;
    [SerializeField] private EnemyUnitTypeCollection spawnableEnemies;
    [SerializeField] private TileCollection shipTiles;
    [SerializeField] private SoulBuffs soulBuffs;
    [SerializeField] private int minSecondarySouls;
    [SerializeField] private int maxSecondarySouls;
    
    private Text _actionHint;
    private GameObject _exfilButton;
    private SceneTerrain _terrain;
    private StatusEffect _encounterBuffType;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _actionHint = transform.Find("ActionHint").GetComponent<Text>();
      _actionHint.gameObject.SetActive(false);
      _exfilButton = transform.Find("ExfilButton").gameObject;
      _exfilButton.SetActive(false);
    }

    private void Start() {
      var soulType = GameState.State.world.GetActiveTile().DownCast<EncounterWorldTile>().soulType;
      _encounterBuffType = soulBuffs.GetBuff(soulType);
    }

    protected override void OnEnable() {
      base.OnEnable();
      Dispatch.Encounters.EncounterStart.RegisterListener(OnEncounterStart);
      Dispatch.Encounters.ItemCollected.RegisterListener(OnItemCollected);
      Dispatch.Encounters.SpawnCollectable.RegisterListener(OnSpawnCollectable);
      Dispatch.Encounters.UnitAddedMidEncounter.RegisterListener(OnUnitAddedMidEncounter);
    }

    protected override void OnDisable() {
      base.OnDisable();
      Dispatch.Encounters.EncounterStart.UnregisterListener(OnEncounterStart);
      Dispatch.Encounters.ItemCollected.UnregisterListener(OnItemCollected);
      Dispatch.Encounters.SpawnCollectable.UnregisterListener(OnSpawnCollectable);
      Dispatch.Encounters.UnitAddedMidEncounter.UnregisterListener(OnUnitAddedMidEncounter);
    }

    private void OnEncounterStart() {
      _actionHint.gameObject.SetActive(true);
      _actionHint.text = getKeyHintText;
      ApplySoulBuff();
      StartCoroutine(FlashObjective());
    }

    private void OnUnitAddedMidEncounter(EncounterActor unit) {
      ApplySoulBuffToUnit(unit);
    }

    private void ApplySoulBuff() {
      foreach (var enemyUnit in enemyUnitsInEncounter) {
        ApplySoulBuffToUnit(enemyUnit);
      }
    }

    private void ApplySoulBuffToUnit(EncounterActor unit) {
      var buff = _encounterBuffType.ApplyTo(unit);
      buff.PreCalculateNoContext();
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
      SpawnReinforcements();
      SpawnSecondaryCollectables();
      StartCoroutine(FlashObjective());
    }

    private void OnSpawnCollectable(Vector3Int position, CollectableInstance collectableInstance) {
      if (!collectableInstance.isPrimaryObjective) {
        return;
      }

      // TODO(P1): Animate and all that.
      Instantiate(spiritKeyPrefab).GetComponent<EncounterCollectable>().Initialize(collectableInstance, position);
    }
    
    private void SpawnSecondaryCollectables() {
      // generate secondary pickups for nearby encounters.
      var encounterTile = GameState.State.world.GetActiveTile();
      var rng = new System.Random();
      foreach (var connectedEncounter in GameState.State.world.GetAllConnectedEncounters(encounterTile)) {
        var collectableInstance = new CollectableInstance {
            isPrimaryObjective = false,
            name = "Soul Fragment",
            contents = new() {
                [connectedEncounter.soulType] =
                    (int)(rng.Next(minSecondarySouls, maxSecondarySouls) * encounterTile.difficulty),
            },
        };
        var destination = _terrain.GetRandomAvailableTile(Vector2Int.one, tile => !shipTiles.Contains(tile));
        var pickup = Instantiate(secondaryPickupPrefab).GetComponent<EncounterCollectable>();
        pickup.Initialize(collectableInstance, destination);
        pickup.DropIn();
      }
    }

    private void SpawnReinforcements() {
      Debug.Log("Spawning reinforcements");
      foreach (var enemy in spawnableEnemies.RandomEnemySpawnsForDifficulty(
          GameState.State.world.GetActiveTile().difficulty)) {
        var destination = _terrain.GetRandomAvailableTile(enemy.size, tile => !shipTiles.Contains(tile));
        var enemyEncounter = enemy.NewEncounter(destination);
        Dispatch.Encounters.SpawnEnemyRequest.Raise(enemyEncounter, 0);
      }
    }
  }
}