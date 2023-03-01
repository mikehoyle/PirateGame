using System;
using State.Unit;
using UnityEngine;

namespace Encounters.Effects {
  [Serializable]
  public class SpawnerStatusEffect : PerRoundStatusEffect {
    // TODO(P1): This all will need to be reworked to work with difficulty ratings etc.
    public int minUnitsToSpawn;
    public int maxUnitsToSpawn;
    public EnemyUnitMetadata unitToSpawn;
    public GameObject enemyUnitPrefab;
    
    public override AppliedStatusEffect ApplyTo(EncounterActor victim) {
      var component = victim.StatusEffects.AddComponent<AppliedSpawnerStatusEffect>();
      component.Initialize(this, victim);
      return component;
    }
  }
}