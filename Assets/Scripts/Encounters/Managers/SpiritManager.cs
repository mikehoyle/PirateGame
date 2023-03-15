using System.Collections;
using System.Collections.Generic;
using Common;
using Encounters.Enemies;
using Encounters.ShipPlacement;
using Optional;
using RuntimeVars.Encounters;
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
    private Option<SpiritUnitController> _pendingSpirit;
    private List<Bones> _unclaimedBones;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _unclaimedBones = new();
      _pendingSpirit = Option.None<SpiritUnitController>();
    }

    private void OnEnable() {
      encounterEvents.encounterStart.RegisterListener(OnEncounterStart);
      encounterEvents.unitDeath.RegisterListener(OnUnitDeath);
    }

    private void OnDisable() {
      encounterEvents.encounterStart.UnregisterListener(OnEncounterStart);
      encounterEvents.unitDeath.UnregisterListener(OnUnitDeath);
    }

    private void OnEncounterStart() {
      _encounterProfile = TerrainProfile.BuildFrom(_terrain.AllTiles);
      _pendingSpirit = Option.Some(NewSpirit());
    }

    private SpiritUnitController NewSpirit() {
      // TODO(P1): Animate spirit spawn.
      Vector3Int position;
      do {
        // This could spin inefficiently in theory, but there should be few enough spirits
        // compared to possibilities that it wouldn't matter.
        position = _encounterProfile.RandomPositionOnBorder();
      } while (SceneTerrain.IsTileOccupied(position));

      var spirit = Instantiate(spiritEnemy.prefab, transform).GetComponent<SpiritUnitController>();
      spirit.Init(spiritEnemy.NewEncounter(position));
      return spirit;
    }

    private void OnUnitDeath(Option<Bones> bonesOption) {
      if (!bonesOption.TryGet(out var bones) || bones.DeadUnit.faction == UnitFaction.PlayerParty) {
        // Unit died but did not drop bones (Therefore no new spirit to chase it).
        // Also, players do not get a spirit allocated to them.
        return;
      }

      _pendingSpirit.Match(
          spirit => {
            spirit.TargetBones = Option.Some(bones);
            _pendingSpirit = Option.None<SpiritUnitController>();
          },
          () => _unclaimedBones.Add(bones)
      );
    }
    
    
    public IEnumerator SpawnSpirits() {
      if (_unclaimedBones.Count == 0) {
        yield break;
      }

      foreach (var bone in _unclaimedBones) {
        var spirit = NewSpirit();
        spirit.TargetBones = Option.Some(bone);
      }

      _pendingSpirit = Option.Some(NewSpirit());
      _unclaimedBones.Clear();
    }
  }
}