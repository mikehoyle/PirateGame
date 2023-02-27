using UnityEngine;

namespace State.Unit {
  [CreateAssetMenu(menuName = "State/EnemyUnitTypeCollection")]
  public class EnemyUnitTypeCollection : ScriptableObject {
    public EnemyUnitMetadata[] enemyUnits;
  }
}