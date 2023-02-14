using State.Unit;
using UnityEngine;

namespace StaticConfig.Sprites {
  [CreateAssetMenu(menuName = "Sprites/DirectionalAnimatedSprite")]
  public class DirectionalAnimatedSprite : ScriptableObject {
    public Sprite[] frames;
    public SerializableDictionary<string, DirectionalAnimation> animations;

    public DirectionalAnimation.Animation? GetAnimation(string name, FacingDirection direction) {
      if (animations.TryGetValue(name, out var animation)) {
        return animation.GetAnimationForDirection(direction);
      }
      
      Debug.LogWarning($"No animation known for name: {name}, and direction {direction}");
      return null;
    }
  }
}