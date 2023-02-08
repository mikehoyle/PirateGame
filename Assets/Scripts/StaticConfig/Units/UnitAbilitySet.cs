using UnityEngine;

namespace StaticConfig.Units {
  [CreateAssetMenu(menuName = "Units/AbilitySet")]
  public class UnitAbilitySet : ScriptableObject {
    public UnitAbility[] abilities;
  }
}