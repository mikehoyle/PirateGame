using System;
using Encounters;
using Encounters.Grid;
using UnityEngine;

namespace Units.Abilities.Range {
  [Serializable]
  public class UniversalRange : AbilityRange {
    protected override bool IsInRangeInternal(EncounterActor actor, Vector3Int source, Vector3Int target) {
      return true;
    }

    public override void DisplayTargetingRange(EncounterActor actor, GridIndicators indicators, Vector3Int source) {
      // For now, just display nothing to avoid spamming entire battlefield.
      return;
    }
  }
}