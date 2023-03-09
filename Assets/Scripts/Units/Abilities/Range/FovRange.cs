using System;
using Common;
using Common.Animation;
using Common.Grid;
using Encounters;
using Encounters.Grid;
using State.Unit;
using UnityEngine;

namespace Units.Abilities.Range {
  /// <summary>
  /// Like <code>SimpleRadiusRange</code>, but limits to 90deg around the actor's facing direction.
  /// </summary>
  [Serializable]
  public class FovRange : AbilityRange {
    public int rangeMin;
    public int rangeMax;

    public override bool IsInRange(EncounterActor actor, Vector3Int source, Vector3Int target) {
      var distance = GridUtils.DistanceBetween(source, target);
      if (distance < rangeMin || distance > rangeMax) {
        return false;
      }

      return actor.FacingDirection.IsInFov(source, target);
    }
    
    public override void DisplayTargetingRange(EncounterActor actor, GridIndicators indicators, Vector3Int source) {
      bool ExcludeFunction(Vector3Int target) => !actor.FacingDirection.IsInFov(source, target);
      indicators.RangeIndicator.DisplayTargetingRangeWithExclusions(source, rangeMin, rangeMax, ExcludeFunction);
    }
  }
}