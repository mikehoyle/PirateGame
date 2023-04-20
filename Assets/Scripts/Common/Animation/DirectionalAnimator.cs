using System;
using Optional;
using UnityEngine;
using UnityEngine.Serialization;

namespace Common.Animation {
  public class DirectionalAnimator : MonoBehaviour {
    [FormerlySerializedAs("sprite")] [SerializeField]
    protected DirectionalAnimatedSprite referenceSprite;
    
    private SpriteRenderer _spriteRenderer;
    
    protected IDirectionalAnimatable AnimationTarget { get; set; }
    
    // State for animation execution
    private DirectionalAnimation.Animation _currentAnimation;
    private Option<Action> _oneOffCallback;
    private float _currentFrameStartTime;
    private int _currentFrame;

    private void Awake() {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      AnimationTarget = GetComponent<IDirectionalAnimatable>();
      if (AnimationTarget == null) {
        Debug.LogWarning($"No animatable component on {name} for DirectionalAnimator to use");
        enabled = false;
      }
      _currentFrame = 0;
    }

    private void OnEnable() {
      AnimationTarget.OneOffAnimation += OnOneOffAnimationRequest;
    }

    private void OnDisable() {
      AnimationTarget.OneOffAnimation -= OnOneOffAnimationRequest;
    }

    private void Update() {
      if (_currentAnimation.direction != AnimationTarget.FacingDirection
          || _currentAnimation.name != AnimationTarget.AnimationState) {
        UpdateAnimationState();
      }

      if (Time.time - _currentFrameStartTime > _currentAnimation.frameDurationSecs) {
        // This technically ignores any overflow time between updates, but it should be negligible.
        _currentFrame++;
        _currentFrameStartTime = Time.time;
        
        if (_currentFrame > _currentAnimation.endFrame) {
          if (_oneOffCallback.TryGet(out var callback)) {
            _oneOffCallback = Option.None<Action>();
            UpdateAnimationState();
            callback();
            return;
          }
          _currentFrame = _currentAnimation.startFrame;
        }

        UpdateSpriteRenderer(_currentFrame, _currentAnimation.isMirrored);
      }
    }

    public void SetSprite(DirectionalAnimatedSprite newSprite) {
      referenceSprite = newSprite;
    }
    
    protected virtual void UpdateSpriteRenderer(int currentFrame, bool isMirrored) {
      _spriteRenderer.sprite = referenceSprite.frames[currentFrame];
      _spriteRenderer.flipX = isMirrored;
    }

    private void UpdateAnimationState() {
      if (_oneOffCallback.HasValue) {
        // Allow one-offs to complete before updating to steady state.
        return;
      }
      
      NewAnimation(AnimationTarget.AnimationState, Option.None<Action>());
    }

    private void OnOneOffAnimationRequest(string animationName, Action onComplete) {
      Debug.Log($"One off animation requested: {animationName}");
      NewAnimation(animationName, Option.Some(onComplete));
    }

    private void NewAnimation(string animationName, Option<Action> oneOffCallback) {
      var newAnimation = referenceSprite.GetAnimation(animationName, AnimationTarget.FacingDirection);
      if (!newAnimation.HasValue) {
        Debug.LogWarning($"No known animation for {animationName}, not updating animation state");
        if (oneOffCallback.TryGet(out var callback)) {
          callback();
        }
        return;
      }
      _currentAnimation = newAnimation.Value;
      _currentFrame = _currentAnimation.startFrame;
      _currentFrameStartTime = Time.time;
      _oneOffCallback = oneOffCallback;
      UpdateSpriteRenderer(_currentFrame, _currentAnimation.isMirrored);
    }
  }
}