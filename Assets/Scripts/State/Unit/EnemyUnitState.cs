using UnityEngine;

namespace State.Unit {
  [CreateAssetMenu(menuName = "State/EnemyUnitState")]
  public class EnemyUnitState : ScriptableObject {
    public int startingHp;
    public int movementRange;
    public UnitEncounterState encounterState;
  }
}