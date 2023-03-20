using Units;
using UnityEngine;

namespace RuntimeVars {
  [CreateAssetMenu(menuName = "Encounters/UnitCollection")]
  public class UnitCollection : CollectionVar<PlayerUnitController> {}
}