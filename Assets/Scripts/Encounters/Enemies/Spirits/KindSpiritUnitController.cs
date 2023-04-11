using StaticConfig.Units;
using UnityEngine;

namespace Encounters.Enemies.Spirits {
  public class KindSpiritUnitController : SpiritUnitController {
    [SerializeField] private int healOnCollision;
    
    protected override void OnPassThroughUnit(EncounterActor victim) {
      victim.ExpendResource(ExhaustibleResources.Instance.hp, -healOnCollision);
    }
  }
}