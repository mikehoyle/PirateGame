using Optional;
using StaticConfig.Builds;
using UnityEngine;

namespace RuntimeVars.ShipBuilder {
  [CreateAssetMenu(menuName = "Vars/ShipBuilder/CurrentBuildSelection")]
  public class CurrentBuildSelection : ScriptableObject {
    public Option<ConstructableObject> build;
    public Option<Vector3Int> tile;
    public bool isValidPlacement;

    public void Clear() {
      build = Option.None<ConstructableObject>();
      tile = Option.None<Vector3Int>();
      isValidPlacement = false;
    }
  }
}