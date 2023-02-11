using Common;
using Encounters;
using Encounters.Effects;
using Encounters.Grid;
using UnityEngine;

namespace Units.Abilities {
  public abstract class RangedAbility : UnitAbility {
    [SerializeField] protected int rangeMin;
    [SerializeField] protected int rangeMax;
    [SerializeField] protected StatusEffect incurredEffect;

    public override void OnSelected(EncounterActor actor, GridIndicators indicators) {
      indicators.RangeIndicator.DisplayTargetingRange(actor.Position, rangeMin, rangeMax);
    }

    protected bool IsInRange(Vector3Int source, Vector3Int target) {
      var distance = GridUtils.DistanceBetween(source, target);
      return distance >= rangeMin && distance <= rangeMax;
    }
  }
}