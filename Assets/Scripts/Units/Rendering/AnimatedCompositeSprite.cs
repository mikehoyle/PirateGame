using System;
using System.Collections.Generic;
using System.Linq;
using StaticConfig;
using UnityEngine;
using static StaticConfig.CompositeUnitSpriteScriptableObject;

namespace Units.Rendering {
  public class AnimatedCompositeSprite : MonoBehaviour, IAnimationListener {
    private readonly Dictionary<Layer, SpriteRenderer> _layerRenderers = new();
    private readonly Dictionary<Layer, CompositeSpriteComponentScriptableObject> _components = new();

    [SerializeField] private CompositeUnitSpriteScriptableObject compositeUnitOptions;
    [SerializeField] private CompositeAnimation[] animations;

    private CompositeAnimation _currentAnimation;

    private void Awake() {
      foreach (var layer in LayerToString) {
        var layerRenderer = new GameObject(layer.Value);
        layerRenderer.AddComponent<SpriteRenderer>();
        layerRenderer.transform.parent = transform;
        layerRenderer.transform.SetAsFirstSibling();
        
        // Default to the first found component for each layer, for now.
        // This will be random, as dictionaries are unordered.
        _components[layer.Key] = compositeUnitOptions.GetOptions(layer.Key).First().Value;
        var renderer = layerRenderer.GetComponent<SpriteRenderer>();
        // Default to the first frame, for now.
        renderer.sprite = _components[layer.Key].frames[0];
        _layerRenderers[layer.Key] = renderer;
      }

      // Default to idle NE animation, for now.
      Play(CompositeAnimation.Type.IdleNe);
    }

    private void Update() {
      _currentAnimation?.Update();
    }

    public void Play(CompositeAnimation.Type animationType) {
      if (_currentAnimation?.type == animationType) {
        // Do nothing on subsequent requests for the same animation
        return;
      }
      
      foreach (var animation in animations) {
        if (animation.type == animationType) {
          _currentAnimation = animation;
          _currentAnimation.SetListener(this);
          _currentAnimation.Reset();
          return;
        }
      }
      
      Debug.LogWarning($"No animation set for {animationType}");
    }
    
    public void OnNewFrame(int frameIndex, bool mirror) {
      foreach (var layerRenderer in _layerRenderers) {
        layerRenderer.Value.sprite = _components[layerRenderer.Key].frames[frameIndex];
        layerRenderer.Value.flipX = mirror;
      }
    }

    public void OnAnimationComplete(CompositeAnimation.Type animationType) {
      // For now, just return to idle every time a one-off animation finishes.
      Play(CompositeAnimation.Type.IdleNe);
    }
  }
}