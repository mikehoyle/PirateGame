using Common;
using UnityEngine;

namespace StaticConfig.RawResources {
  /// <summary>
  /// Represents a single raw resource available in-world.
  /// </summary>
  [CreateAssetMenu(menuName = "Config/RawResource")]
  public class RawResource : EnumScriptableObject {
    public string displayName;
    public string spriteTag;
  }
}