using UnityEngine;

namespace StaticConfig {
  /// <summary>
  /// Represents all build options available in the game.
  /// </summary>
  [CreateAssetMenu(fileName = "AllBuildOptions", menuName = "ScriptableObjects/AllBuildOptions", order = 0)]
  public class AllBuildOptionsScriptableObject : ScriptableObject {
    public ConstructableScriptableObject[] buildOptions;
  }
}