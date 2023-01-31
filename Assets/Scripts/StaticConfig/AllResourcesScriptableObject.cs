using UnityEngine;

namespace StaticConfig {
  /// <summary>
  /// Represents all resources available in the game.
  /// </summary>
  [CreateAssetMenu(fileName = "AllResources", menuName = "ScriptableObjects/AllResources", order = 0)]
  public class AllResourcesScriptableObject : ScriptableObject {
    public RawResourceScriptableObject[] resources;
  }
}