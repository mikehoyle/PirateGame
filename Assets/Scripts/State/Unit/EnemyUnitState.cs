using UnityEngine;

namespace State.Unit {
  [CreateAssetMenu(menuName = "State/EnemyUnitState")]
  public class EnemyUnitState : ScriptableObject {
    // More will go here
    public UnitEncounterState encounterState;
  }
}