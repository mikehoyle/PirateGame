using Common;
using UnityEngine;

namespace StaticConfig.Units {
  [CreateAssetMenu(menuName = "Units/Stat")]
  public class Stat : EnumScriptableObject {
    public string displayName;
    public int maxValue;
    public int minValue;
  }
}