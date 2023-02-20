using Terrain;
using UnityEngine;

namespace Encounters.Effects {
  [CreateAssetMenu(menuName = "Effects/SpawnerStatusEffect")]
  public class SpawnerStatusEffect : PerRoundStatusEffect {
    

    protected override void EnactEffect(EncounterActor victim) {
      base.EnactEffect(victim);

      var terrain = SceneTerrain.Get();
      if (terrain == null) {
        Debug.LogWarning("Cannot spawn units, no known terrain");
      }

      
    }
  }
}