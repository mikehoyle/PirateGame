using Common;
using UnityEngine;

namespace StaticConfig.RawResources {
  /// <summary>
  /// Represents a single raw resource available in-world.
  /// </summary>
  [CreateAssetMenu(fileName = "RawResource", menuName = "ScriptableObjects/RawResource", order = 0)]
  public class RawResource : EnumScriptableObject {
    public string displayName;
  }
}