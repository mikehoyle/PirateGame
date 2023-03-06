using System;
using Encounters;
using Encounters.Grid;
using UnityEngine;

namespace Units.Abilities.Range {
  [Serializable]
  public class StraightLineRange : AbilityRange {
    [SerializeField] private int diagonalRangeMin;
    [SerializeField] private int diagonalRangeMax;
    [SerializeField] private int straightRangeMin;
    [SerializeField] private int straightRangeMax;
    
    public override bool IsInRange(EncounterActor _, Vector3Int source, Vector3Int target) {
      var difference = target - source;
      var xDiff = Math.Abs(difference.x);
      var yDiff = Math.Abs(difference.y);
      if (diagonalRangeMax > 0) {
        if (xDiff == yDiff && xDiff >= diagonalRangeMin && xDiff <= diagonalRangeMax) {
          return true;
        }
      }

      if (straightRangeMax > 0) {
        if (xDiff == 0 && yDiff >= straightRangeMin && yDiff <= straightRangeMax) {
          return true;
        }
        if (yDiff == 0 && xDiff >= straightRangeMin && xDiff <= straightRangeMax) {
          return true;
        }
      }

      return false;
    }

    public override void DisplayTargetingRange(EncounterActor _, GridIndicators indicators, Vector3Int source) {
      indicators.RangeIndicator.DisplayStraightLineRange(
          source, diagonalRangeMin, diagonalRangeMax, straightRangeMin, straightRangeMax);
    }
  }
}