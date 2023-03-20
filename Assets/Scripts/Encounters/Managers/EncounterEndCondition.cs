using System.Linq;
using Optional;
using RuntimeVars;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using Units;
using UnityEngine;

namespace Encounters.Managers {
  public class EncounterEndCondition : MonoBehaviour {
    [SerializeField] protected EncounterEvents encounterEvents;
    [SerializeField] protected UnitCollection playerUnitsInEncounter;
    [SerializeField] protected EnemyUnitCollection enemyUnitsInEncounter;

    protected virtual void OnEnable() {
      encounterEvents.unitDeath.RegisterListener(OnUnitDeath);
    }
    
    protected virtual void OnDisable() {
      encounterEvents.unitDeath.RegisterListener(OnUnitDeath);
    }
    
    // Default loss condition = all player units dead.
    protected virtual void OnUnitDeath(Option<Bones> _) {
      if (!playerUnitsInEncounter.Any()) {
        encounterEvents.encounterEnd.Raise(EncounterOutcome.PlayerDefeat);
      }
    }
  }
}