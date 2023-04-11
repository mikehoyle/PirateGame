﻿using System.Collections.Generic;
using System.Linq;
using Common;
using Encounters.Enemies.Spirits;
using Encounters.ShipPlacement;
using Events;
using Optional;
using State;
using State.Unit;
using State.World;
using StaticConfig.RawResources;
using Terrain;
using Units;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Encounters.Managers {
  public class SpiritManager : MonoBehaviour {
    [SerializeField] private EnemyUnitMetadata kindSpiritEnemy;
    [SerializeField] private EnemyUnitMetadata diligentSpiritEnemy;
    [SerializeField] private EnemyUnitMetadata treacherousSpiritEnemy;
    [SerializeField] private EnemyUnitMetadata violentSpiritEnemy;
    
    private SceneTerrain _terrain;
    private TerrainProfile _encounterProfile;
    private List<Bones> _pendingBones;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _pendingBones = new();
    }

    private void OnEnable() {
      Dispatch.Encounters.EncounterStart.RegisterListener(OnEncounterStart);
      Dispatch.Encounters.UnitDeath.RegisterListener(OnUnitDeath);
      Dispatch.Encounters.EnemyTurnPreEnd.RegisterListener(OnEnemyTurnPreEnd);
    }

    private void OnDisable() {
      Dispatch.Encounters.EncounterStart.UnregisterListener(OnEncounterStart);
      Dispatch.Encounters.UnitDeath.UnregisterListener(OnUnitDeath);
      Dispatch.Encounters.EnemyTurnPreEnd.UnregisterListener(OnEnemyTurnPreEnd);
    }

    private void OnEncounterStart() {
      _encounterProfile = TerrainProfile.BuildFrom(_terrain.AllTiles);
    }

    private void CreateNewSpirit(Bones targetBones) {
      // TODO(P1): Animate spirit spawn.
      var rng = new Random();
      var possibleSpawns = new List<Vector3Int>();
      possibleSpawns.Add(
          new Vector3Int(targetBones.Position.x, _encounterProfile.TopEdge[targetBones.Position.x] + 1));
      possibleSpawns.Add(
          new Vector3Int(targetBones.Position.x, _encounterProfile.BottomEdge[targetBones.Position.x] - 1));
      possibleSpawns.Add(
          new Vector3Int(_encounterProfile.RightEdge[targetBones.Position.y] + 1, targetBones.Position.y));
      possibleSpawns.Add(
          new Vector3Int(_encounterProfile.LeftEdge[targetBones.Position.y] - 1, targetBones.Position.y));
      
      Vector3Int position = default;
      bool foundValidPosition = false;
      foreach (var possibleSpawn in possibleSpawns.OrderBy(_ => rng.Next())) {
        if (!SceneTerrain.IsTileOccupied(possibleSpawn)) {
          position = possibleSpawn;
          foundValidPosition = true;
          break;
        }
      }

      if (!foundValidPosition) {
        Debug.LogWarning($"No valid spawn point for spirit targeting {targetBones.Position}, not spawning");
        return;
      }

      var encounterSoulType = GameState.State.world.GetActiveTile().DownCast<EncounterWorldTile>().soulType;

      EnemyUnitMetadata spiritType;
      if (encounterSoulType == SoulTypes.Instance.kind) {
        spiritType = kindSpiritEnemy;
      } else if (encounterSoulType == SoulTypes.Instance.diligent) {
        spiritType = diligentSpiritEnemy;
      } else if (encounterSoulType == SoulTypes.Instance.treacherous) {
        spiritType = treacherousSpiritEnemy;
      } else {
        spiritType = violentSpiritEnemy;
      }

      var spirit = Instantiate(spiritType.prefab, transform).GetComponent<SpiritUnitController>();
      spirit.Init(spiritType.NewEncounter(position));
      spirit.TargetBones = Option.Some(targetBones);
    }

    private void OnUnitDeath(Option<Bones> bonesOption) {
      if (!bonesOption.TryGet(out var bones) || bones.DeadUnit.faction == UnitFaction.PlayerParty) {
        // Unit died but did not drop bones (Therefore no new spirit to chase it).
        // Also, players do not get a spirit allocated to them.
        return;
      }

      _pendingBones.Add(bones);
    }

    private void OnEnemyTurnPreEnd() {
      foreach (var bones in _pendingBones) {
        CreateNewSpirit(bones);
      }
      _pendingBones.Clear();
    }
  }
}