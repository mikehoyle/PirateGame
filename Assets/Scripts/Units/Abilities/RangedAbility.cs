using System.Collections.Generic;
using Common;
using Encounters;
using Encounters.Grid;
using State.Unit;
using Units.Abilities.Range;
using UnityEngine;

namespace Units.Abilities {
  public abstract class RangedAbility : UnitAbility {
    [SerializeReference, SerializeReferenceButton] private AbilityRange range;
    [SerializeField] protected bool canTargetOpponents = true;
    [SerializeField] protected bool canTargetAllies = false;

    public override void ShowIndicator(
        EncounterActor actor,
        Vector3Int source,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      range.DisplayTargetingRange(actor, indicators, source);
    }

    protected AbilityRange GetRange(EncounterActor actor) {
      if (actor is not PlayerUnitController playerUnit) {
        return range;
      }

      var currentTier = 0;
      var result = range;
      foreach (var upgrade in playerUnit.Metadata.GetAllUpgrades()) {
        // Sloppy logic that enforces at least higher-tier upgrades always override lower-tier upgrades.
        if (upgrade.GetUpgradeTier() > currentTier && upgrade.GetRangeOverride(playerUnit.Metadata, this).TryGet(out var rangeOverride)) {
          currentTier = upgrade.GetUpgradeTier();
          result = rangeOverride;
        }
      }
      return result;
    }

    protected List<UnitFaction> GetAffectedFactions(EncounterActor actor) {
      var affectedFactions = new List<UnitFaction>();
      if (canTargetAllies) {
        affectedFactions.Add(actor.EncounterState.faction);
      }
      if (canTargetOpponents) {
        affectedFactions.Add(actor.EncounterState.OpposingFaction());
      }
      return affectedFactions;
    }

    public static List<UnitFaction> AllFactions() {
      return new List<UnitFaction> {
          UnitFaction.Enemy,
          UnitFaction.PlayerParty,
      };
    }
  }
}