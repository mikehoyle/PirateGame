using System;
using State.Unit;

namespace Common.Animation {
  public interface IDirectionalAnimatable {
    public delegate void RequestOneOffAnimation(string animationName);
    
    public FacingDirection FacingDirection { get; }
    public string AnimationState { get; }
    
    public event RequestOneOffAnimation OneOffAnimation;
  }
}