using StaticConfig.Units;
using UnityEngine;

namespace State.Enemy {
  [CreateAssetMenu(menuName = "State/EnemyUnitEncounterState")]
  public class EnemyUnitEncounterState : ScriptableObject {
    public ExhaustibleResourceTracker[] resources;
    public Vector3Int position;

    // TODO(P0): much more, this is just a stub
  }
}