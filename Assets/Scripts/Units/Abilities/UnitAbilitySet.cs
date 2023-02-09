using UnityEngine;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/AbilitySet")]
  public class UnitAbilitySet : ScriptableObject {
    public UnitAbility[] abilities;
  }
}