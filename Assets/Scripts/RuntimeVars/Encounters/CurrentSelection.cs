using Optional;
using Optional.Unsafe;
using StaticConfig.Units;
using Units;
using UnityEngine;

namespace RuntimeVars.Encounters {
  [CreateAssetMenu(menuName = "Encounters/CurrentSelection")]
  public class CurrentSelection : ScriptableObject {
    public Option<UnitAbility> selectedAbility;
    public Option<UnitController> selectedUnit;

    public bool TryGet(out UnitAbility ability, out UnitController unit) {
      if (selectedAbility.HasValue && selectedUnit.HasValue) {
        ability = selectedAbility.ValueOrFailure();
        unit = selectedUnit.ValueOrFailure();
        return true;
      }
      ability = null;
      unit = null;
      return false;
    } 
    
    public void Reset() {
      selectedAbility = Option.None<UnitAbility>();
      selectedUnit = Option.None<UnitController>();
    }
  }
}