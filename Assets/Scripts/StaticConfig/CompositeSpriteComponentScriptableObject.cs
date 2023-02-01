using UnityEngine;

namespace StaticConfig {
  [CreateAssetMenu(fileName = "CompositeSpriteComponent", menuName = "ScriptableObjects/Composite Sprite Component", order = 0)]
  public class CompositeSpriteComponentScriptableObject : ScriptableObject {
    public Sprite[] frames;
  }
}