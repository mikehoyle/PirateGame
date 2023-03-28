using UnityEngine;

namespace Common.Animation {
  [CreateAssetMenu(menuName = "ScriptableObjects/ColorCollection")]
  public class ColorCollection : ScriptableObject {
    public Color[] colors;
  }
}