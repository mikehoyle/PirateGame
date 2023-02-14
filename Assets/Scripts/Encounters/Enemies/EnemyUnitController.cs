using System.Collections.Generic;
using System.Linq;
using Encounters.Grid;
using RuntimeVars.Encounters;
using State.Unit;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Enemies {
  public class EnemyUnitController : EncounterActor {
    [SerializeField] private EnemyUnitCollection enemiesInEncounter;
    [SerializeField] private UnitAbilitySet defaultAbilities;

    public override UnitEncounterState EncounterState { get; protected set; }

    protected override void OnEnable() {
      base.OnEnable();
      enemiesInEncounter.Add(this);
      encounterEvents.playerTurnStart.RegisterListener(OnNewRound);
    }

    protected override void OnDisable() {
      base.OnDisable();
      enemiesInEncounter.Remove(this);
      encounterEvents.playerTurnStart.UnregisterListener(OnNewRound);
    }
    
    public List<UnitAbility> GetAllCapableAbilities() {
      var result = defaultAbilities.abilities.ToList();
      return result;
    }

    private void OnNewRound() {
      EncounterState.NewRound();
    }

    public void Init(EnemyUnitState state) {
      EncounterState = state.encounterState;
      // TODO(P0): prototyping only, remove this
      EncounterState.resources = new[] {
          ExhaustibleResourceTracker.NewHpTracker(10),
          ExhaustibleResourceTracker.NewMovementTracker(5),
          ExhaustibleResourceTracker.NewActionPointsTracker(2), 
      };
    }
  }
}