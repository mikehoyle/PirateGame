using System;
using System.Linq;
using State.Unit;
using UnityEngine;

namespace StaticConfig.Sprites {
  [Serializable]
  public class DirectionalAnimation {
    [Serializable]
    public struct Animation {
      public FacingDirection direction;
      public string name;
      public bool isMirrored;
      public int startFrame;
      public int endFrame;
      public float frameDurationSecs;

      public Animation(
          FacingDirection direction,
          string name,
          int startFrame,
          int endFrame,
          float frameDurationSecs,
          bool isMirrored = false) {
        this.direction = direction;
        this.name = name;
        this.isMirrored = isMirrored;
        this.frameDurationSecs = frameDurationSecs;
        this.startFrame = startFrame;
        this.endFrame = endFrame;
      } 

      public Animation Mirrored() {
        return new Animation(
            GetMirroredDirection(direction), name, startFrame, endFrame, frameDurationSecs, !isMirrored);
      }
    }
    
    public SerializableDictionary<FacingDirection, Animation> animations;
    public string animationName;
    // Aseprite doesn't indicate this, so it's just for manual setting after import.
    public bool loop;

    private DirectionalAnimation() {
      // For Unity serializer
      animations = new();
    }
    
    public DirectionalAnimation(string animationName) {
      this.animationName = animationName;
      animations = new();
      loop = true;
    }

    public void AddAnimation(
        FacingDirection facingDirection, int startFrame, int endFrame, float frameDurationSecs) {
      animations[facingDirection] = new Animation(
          facingDirection,
          animationName,
          startFrame,
          endFrame,
          frameDurationSecs);
    }

    public Animation GetAnimationForDirection(FacingDirection direction) {
      if (animations.TryGetValue(direction, out var animation)) {
        return animation;
      }
      
      if (animations.TryGetValue(GetMirroredDirection(direction), out var mirroredAnimation)) {
        return mirroredAnimation.Mirrored();
      }
      
      Debug.LogWarning($"No animation found for the direction {direction}");
      return animations.First().Value;
    }

    public static FacingDirection GetMirroredDirection(FacingDirection direction) {
      return direction switch {
          FacingDirection.NorthEast => FacingDirection.NorthWest,
          FacingDirection.SouthEast => FacingDirection.SouthWest,
          FacingDirection.NorthWest => FacingDirection.NorthEast,
          FacingDirection.SouthWest => FacingDirection.SouthEast,
          _ => FacingDirection.NorthEast,
      };
    }
  }
}