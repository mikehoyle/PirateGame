﻿using System.Collections;
using System.Collections.Generic;
using Common;
using Encounters.Enemies;
using Encounters.ShipPlacement;
using Events;
using Optional;
using RuntimeVars.Encounters;
using State.Unit;
using Terrain;
using Units;
using UnityEngine;

namespace Encounters.Managers {
  public class SpiritManager : MonoBehaviour {
    [SerializeField] private EnemyUnitMetadata spiritEnemy;
    
    private SceneTerrain _terrain;
    private TerrainProfile _encounterProfile;

    private void Awake() {
      _terrain = SceneTerrain.Get();
    }

    private void OnEnable() {
      Dispatch.Encounters.EncounterStart.RegisterListener(OnEncounterStart);
      Dispatch.Encounters.UnitDeath.RegisterListener(OnUnitDeath);
    }

    private void OnDisable() {
      Dispatch.Encounters.EncounterStart.UnregisterListener(OnEncounterStart);
      Dispatch.Encounters.UnitDeath.UnregisterListener(OnUnitDeath);
    }

    private void OnEncounterStart() {
      _encounterProfile = TerrainProfile.BuildFrom(_terrain.AllTiles);
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

      NewSpirit().TargetBones = Option.Some(bones);
    }
  }
}