using System;
using Common.Grid;
using Encounters;
using Encounters.Grid;
using UnityEngine;

namespace Units.Abilities.Range {
  [Serializable]
  public class SimpleRadiusRange : AbilityRange {
    public int rangeMin;
    public int rangeMax;

    public override bool IsInRange(EncounterActor actor, Vector3Int source, Vector3Int target) {
      var distance = GridUtils.DistanceBetween(source, target);
      return distance >= rangeMin && distance <= rangeMax;
    }

    public override void DisplayTargetingRange(EncounterActor actor, GridIndicators indicators, Vector3Int source) {
      indicators.RangeIndicator.DisplayTargetingRange(source, rangeMin, rangeMax);
    }
  }
}