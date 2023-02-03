using System;
using System.Collections.Generic;
using System.Linq;
using State;
using StaticConfig;
using UnityEngine;
using static StaticConfig.CompositeUnitSpriteScriptableObject;

namespace Units.Rendering {
  public class AnimatedCompositeSprite : MonoBehaviour, IAnimationListener {
    private readonly Dictionary<Layer, SpriteRenderer> _layerRenderers = new();
    private readonly Dictionary<Layer, CompositeSpriteComponentScriptableObject> _components = new();

    [SerializeField] private CompositeUnitSpriteScriptableObject compositeUnitOptions;
    [SerializeField] private Material paletteSwapMaterial;
    [SerializeField] private CompositeAnimation[] animations;

    private CompositeAnimation _currentAnimation;

    private void Awake() {
      foreach (var layer in LayerToString) {
        var layerRenderer = new GameObject(layer.Value);
        var spriteRenderer = layerRenderer.AddComponent<SpriteRenderer>();
        layerRenderer.transform.parent = transform;
        layerRenderer.transform.SetAsFirstSibling();
        
        
        // Default to the first found component for each layer, for now.
        // This will be random, as dictionaries are unordered.
        _components[layer.Key] = compositeUnitOptions.GetOptions(layer.Key).First().Value;
        // Default to the first frame, for now.
        spriteRenderer.sprite = _components[layer.Key].frames[0];
        spriteRenderer.material = paletteSwapMaterial;
        _layerRenderers[layer.Key] = spriteRenderer;
      }

      // Default to idle NE animation, for now.
      Play(CompositeAnimation.Type.IdleNe);
    }

    private void Update() {
      _currentAnimation?.Update();
    }

    // For now, wholesale change color so faction is apparent
    public void SetColorForFaction(UnitFaction faction) {
      var targetColor = faction switch {
          // greenish
          UnitFaction.PlayerParty => new Color(5/255f, 100/255f, 45/255f),
          // redish
          _ => new Color(190/255f, 100/255f, 45/255f),
      };

      foreach (var spriteRenderer in _layerRenderers.Values) {
        spriteRenderer.material.SetColor("_ShirtColor", targetColor);
      }
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