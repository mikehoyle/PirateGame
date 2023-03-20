using System.Linq;
using Events;
using Optional;
using RuntimeVars;
using RuntimeVars.Encounters;
using Units;
using UnityEngine;

namespace Encounters.Managers {
  public class EncounterType : MonoBehaviour {
    [SerializeField] protected UnitCollection playerUnitsInEncounter;
    [SerializeField] protected EnemyUnitCollection enemyUnitsInEncounter;

    protected virtual void OnEnable() {
      Dispatch.Encounters.UnitDeath.RegisterListener(OnUnitDeath);
    }
    
    protected virtual void OnDisable() {
      Dispatch.Encounters.UnitDeath.RegisterListener(OnUnitDeath);
    }
    
    // Default loss condition = all player units dead.
    protected virtual void OnUnitDeath(Option<Bones> _) {
      if (!playerUnitsInEncounter.Any()) {
        Dispatch.Encounters.EncounterEnd.Raise(EncounterOutcome.PlayerDefeat);
      }
    }
  }
}