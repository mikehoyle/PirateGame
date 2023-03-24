using Encounters;
using Encounters.Grid;
using Units.Abilities.Range;
using UnityEngine;

namespace Units.Abilities {
  public abstract class RangedAbility : UnitAbility {
    [SerializeReference, SerializeReferenceButton] protected AbilityRange range;
    [SerializeField] protected bool canTargetOpponents = true;
    [SerializeField] protected bool canTargetAllies = false;

    public override void ShowIndicator(
        EncounterActor actor,
        Vector3Int source,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      range.DisplayTargetingRange(actor, indicators, source);
    }
  }
}