using System.Collections.Generic;
using Common;
using Encounters.Enemies;
using Encounters.ShipPlacement;
using Optional;
using RuntimeVars.Encounters.Events;
using State.Unit;
using Terrain;
using Units;
using UnityEngine;

namespace Encounters.Managers {
  public class SpiritManager : MonoBehaviour {
    [SerializeField] private EnemyUnitMetadata spiritEnemy;
    [SerializeField] private EncounterEvents encounterEvents;
    
    private SceneTerrain _terrain;
    private TerrainProfile _encounterProfile;
    private SpiritUnitController _pendingSpirit;
    private List<Bones> _unclaimedBones;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _unclaimedBones = new();
    }

    private void OnEnable() {
      encounterEvents.encounterStart.RegisterListener(OnEncounterStart);
      encounterEvents.unitDeath.RegisterListener(OnUnitDeath);
      encounterEvents.enemyTurnPreEnd.RegisterListener(OnEnemyTurnPreEnd);
    }

    private void OnDisable() {
      encounterEvents.encounterStart.UnregisterListener(OnEncounterStart);
      encounterEvents.unitDeath.UnregisterListener(OnUnitDeath);
      encounterEvents.enemyTurnPreEnd.UnregisterListener(OnEnemyTurnPreEnd);
    }

    private void OnEncounterStart() {
      _encounterProfile = TerrainProfile.BuildFrom(_terrain.AllTiles);
      NewPendingSpirit();
    }

    private void NewPendingSpirit() {
      // TODO(P0): Make sure spirits can't appear on each other.
      _pendingSpirit = Instantiate(spiritEnemy.prefab, transform).GetComponent<SpiritUnitController>();
      _pendingSpirit.Init(spiritEnemy.NewEncounter(_encounterProfile.RandomPositionOnBorder()));
    }

    private void OnUnitDeath(Option<Bones> bonesOption) {
      if (!bonesOption.TryGet(out var bones)) {
        // Unit died but did not drop bones (Therefore no new spirit to chase it.
        return;
      }

      _unclaimedBones.Add(bones);
    }
    
    
    private void OnEnemyTurnPreEnd() {
      if (_unclaimedBones.Count == 0) {
        return;
      }
      Debug.Log($"Number of unclaimed bones: {_unclaimedBones.Count}");
      _pendingSpirit.TargetBones = Option.Some(_unclaimedBones[0]);
      _unclaimedBones.RemoveAt(0);
      NewPendingSpirit();
    }
  }
}