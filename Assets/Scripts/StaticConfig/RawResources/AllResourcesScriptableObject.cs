using UnityEngine;

namespace StaticConfig.RawResources {
  /// <summary>
  /// Represents all resources available in the game.
  /// </summary>
  [CreateAssetMenu(fileName = "AllResources", menuName = "ScriptableObjects/AllResources", order = 0)]
  public class AllResourcesScriptableObject : ScriptableObject {
    public RawResource[] resources;
  }
}