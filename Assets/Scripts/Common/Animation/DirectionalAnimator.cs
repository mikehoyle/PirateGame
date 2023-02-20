using StaticConfig.Sprites;
using UnityEngine;

namespace Common.Animation {
  public class DirectionalAnimator : MonoBehaviour {
    [SerializeField] private DirectionalAnimatedSprite sprite;
    
    private SpriteRenderer _spriteRenderer;
    private IDirectionalAnimatable _animationTarget;
    
    // State for animation execution
    private DirectionalAnimation.Animation _currentAnimation;
    private bool _currentAnimationIsOneOff;
    private float _currentFrameStartTime;
    private int _currentFrame;

    private void Awake() {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _animationTarget = GetComponent<IDirectionalAnimatable>();
      if (_animationTarget == null) {
        Debug.LogWarning($"No animatable component on {name} for DirectionalAnimator to use");
        enabled = false;
      }
      _currentFrame = 0;
    }

    private void OnEnable() {
      _animationTarget.OneOffAnimation += OnOneOffAnimationRequest;
    }

    private void OnDisable() {
      _animationTarget.OneOffAnimation -= OnOneOffAnimationRequest;
    }

    private void Update() {
      if (_currentAnimation.direction != _animationTarget.FacingDirection
          || _currentAnimation.name != _animationTarget.AnimationState) {
        UpdateAnimationState();
      }

      if (Time.time - _currentFrameStartTime > _currentAnimation.frameDurationSecs) {
        // This technically ignores any overflow time between updates, but it should be negligible.
        _currentFrame++;
        _currentFrameStartTime = Time.time;
        
        if (_currentFrame > _currentAnimation.endFrame) {
          if (_currentAnimationIsOneOff) {
            _currentAnimationIsOneOff = false;
            UpdateAnimationState();
            return;
          }
          _currentFrame = _currentAnimation.startFrame;
        }

        UpdateSpriteRenderer();
      }
    }

    public void SetSprite(DirectionalAnimatedSprite newSprite) {
      sprite = newSprite;
    }
    
    private void UpdateSpriteRenderer() {
      _spriteRenderer.sprite = sprite.frames[_currentFrame];
      _spriteRenderer.flipX = _currentAnimation.isMirrored;
    }

    private void UpdateAnimationState() {
      if (_currentAnimationIsOneOff) {
        // Allow one-offs to complete before updating to steady state.
        return;
      }
      
      NewAnimation(_animationTarget.AnimationState, isOneOff: false);
    }

    private void OnOneOffAnimationRequest(string animationName) {
      Debug.Log($"One off animation requested: {animationName}");
      NewAnimation(animationName, isOneOff: true);
    }

    private void NewAnimation(string animationName, bool isOneOff) {
      var newAnimation = sprite.GetAnimation(animationName, _animationTarget.FacingDirection);
      if (!newAnimation.HasValue) {
        Debug.LogWarning($"No known animation for {animationName}, not updating animation state");
        return;
      }
      _currentAnimation = newAnimation.Value;
      _currentFrame = _currentAnimation.startFrame;
      _currentFrameStartTime = Time.time;
      _currentAnimationIsOneOff = isOneOff;
      UpdateSpriteRenderer();
    }
  }
}