using System.Collections.Generic;
using System.Linq;
using Encounters.Enemies;
using State.Unit;
using UnityEngine;

namespace RuntimeVars.Encounters {
  [CreateAssetMenu(menuName = "Encounters/EnemyUnitCollection")]
  public class EnemyUnitCollection : CollectionVar<EnemyUnitController> {
    public IEnumerable<EnemyUnitController> EnumerateByTurnPriority() {
      return Items.OrderBy(unit => ((EnemyUnitMetadata)unit.EncounterState.metadata).turnPriority);
    }
  }
}