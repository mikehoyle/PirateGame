using UnityEngine;

namespace Common.Animation {
  [CreateAssetMenu(menuName = "Config/ColorCollection")]
  public class ColorCollection : ScriptableObject {
    public Color[] colors;
  }
}