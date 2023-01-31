using UnityEngine;

namespace StaticConfig {
  /// <summary>
  /// Represents a single raw resource available in-world.
  /// </summary>
  [CreateAssetMenu(fileName = "RawResource", menuName = "ScriptableObjects/RawResource", order = 0)]
  public class RawResourceScriptableObject : ScriptableObject {
    public string id;
    public string displayName;
  }
}