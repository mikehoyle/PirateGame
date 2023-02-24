using System;
using Encounters;
using Encounters.Grid;
using UnityEngine;

namespace Units.Abilities.Range {
  [Serializable]
  public abstract class AbilityRange {
    public abstract bool IsInRange(EncounterActor actor, Vector3Int source, Vector3Int target);
    public abstract void DisplayTargetingRange(EncounterActor actor, GridIndicators indicators, Vector3Int source);
  }
}