using State.Unit;
using StaticConfig.Units;
using Units;
using UnityEngine;

namespace RuntimeVars.Encounters {
  [CreateAssetMenu(menuName = "Encounters/CurrentSelection")]
  public class CurrentSelection : ScriptableObject {
    public Vector3Int selectedTile;
    public UnitAbility selectedAbility;
    public UnitController selectedUnit;

    public void Reset() {
      selectedTile = Vector3Int.zero;
      selectedAbility = null;
      selectedUnit = null;
    }
  }
}