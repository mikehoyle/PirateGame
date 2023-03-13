using System;
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
    private SpiritUnitController _pendingSpirit;
    private TerrainProfile _encounterProfile;

    private void Awake() {
      _terrain = SceneTerrain.Get();
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
      NewPendingSpirit();
    }

    private void NewPendingSpirit() {
      _pendingSpirit = Instantiate(spiritEnemy.prefab).GetComponent<SpiritUnitController>();
      _pendingSpirit.Init(spiritEnemy.NewEncounter(_encounterProfile.RandomPositionOnBorder()));
    }

    private void OnUnitDeath(Option<Bones> bones) {
      
    }
  }
}