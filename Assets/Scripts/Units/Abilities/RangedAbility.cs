using Encounters;
using Encounters.Grid;
using Units.Abilities.Range;
using UnityEngine;

namespace Units.Abilities {
  public abstract class RangedAbility : UnitAbility {
    [SerializeReference, SerializeReferenceButton] protected AbilityRange range;
    [SerializeField] protected bool canTargetOpponents = true;
    [SerializeField] protected bool canTargetAllies = false;

    public override void OnSelected(EncounterActor actor, GridIndicators indicators, Vector3Int source) {
      range.DisplayTargetingRange(actor, indicators, source);
    }
  }
}