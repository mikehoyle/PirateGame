using System;
using UnityEngine;

namespace Units.Rendering {
  [Serializable]
  public class CompositeAnimation {
    public enum Type {
      IdleNe,
      WalkNe,
      WalkEquipNe,
      IdleCombatNe,
      AttackSwordNe,
      IdleSw,
      WalkSw,
      WalkEquipSw,
      WalkSe,
      WalkNw,
    }

    public Type type;
    [SerializeField] private int startFrame;
    [SerializeField] private int endFrame;
    [SerializeField] private float intervalSeconds;
    [SerializeField] private bool loop;
    [SerializeField] private bool mirrorSprite;

    private int _currentFrame;
    private float _frameTimeElapsed;
    private bool _animationComplete;
    private IAnimationListener _animationListener;

    /// <summary>
    /// Not intended to be manually instantiated, but will be called by Unity deserializer.
    /// </summary>
    private CompositeAnimation() {
      Reset();
    }

    public void Update() {
      if (_animationComplete) {
        return;
      }
      
      _frameTimeElapsed += Time.deltaTime;
      if (_frameTimeElapsed >= intervalSeconds) {
        // Time for new frame
        _frameTimeElapsed -= intervalSeconds;
        _currentFrame++;
        
        if (_currentFrame > endFrame) {
          if (loop) {
            _currentFrame = startFrame;
          } else {
            _animationComplete = true;
            _currentFrame = endFrame;
            _animationListener?.OnAnimationComplete(type);
            return;
          }
        }
        
        _animationListener?.OnNewFrame(_currentFrame, mirrorSprite);
      }
    }

    public void Reset() {
      _currentFrame = startFrame;
      _frameTimeElapsed = 0;
      _animationComplete = false;
      _animationListener?.OnNewFrame(_currentFrame, mirrorSprite);
    }
    public void SetListener(IAnimationListener animationListener) {
      _animationListener = animationListener;
    }
  }
}