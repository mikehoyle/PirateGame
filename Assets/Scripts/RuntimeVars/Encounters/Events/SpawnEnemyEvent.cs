using Common.Events;
using State.Unit;
using UnityEngine;

namespace RuntimeVars.Encounters.Events {
  /// <summary>
  /// Where the int is the number of rounds until the spawn occurs.
  /// </summary>
  [CreateAssetMenu(menuName = "Events/Encounters/SpawnEnemy")]
  public class SpawnEnemyEvent : ParameterizedGameEvent<UnitEncounterState, int> {}
}