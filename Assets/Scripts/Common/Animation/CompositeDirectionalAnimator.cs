using System;
using System.Collections.Generic;
using StaticConfig.Sprites;
using UnityEngine;

namespace Common.Animation {
  public class CompositeDirectionalAnimator : DirectionalAnimator {
    [SerializeField] private Material paletteSwapMaterial;
    [SerializeField] private DirectionalAnimatedSprite[] layersBackToFront;
    
    private List<SpriteRenderer> _layerRenderers;

    private void Awake() {
      AnimationTarget = GetComponent<IDirectionalAnimatable>();
      // We assume that all composite sprites have the same animations, so just use the first
      // (arbitrarily) as the reference for animations.
      referenceSprite = layersBackToFront[0];
      _layerRenderers = new();
      foreach (var layer in layersBackToFront) {
        var layerRenderer = new GameObject();
        var spriteRenderer = layerRenderer.AddComponent<SpriteRenderer>();
        layerRenderer.transform.parent = transform;
        layerRenderer.transform.SetAsFirstSibling();

        // Default to the first frame, for now.
        spriteRenderer.sprite = layer.frames[0];
        spriteRenderer.material = paletteSwapMaterial;
        _layerRenderers.Add(spriteRenderer);
      }
    }

    // For now, random colors just to differentiate the units
    public void SetColor(string unitName) {
      // hacky way for consistent colors, just seed based on unit's name hash
      var rng = new System.Random(unitName.GetHashCode());
      var targetColor = new Color(
          (float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble(), 1f);
      foreach (var spriteRenderer in _layerRenderers) {
        spriteRenderer.material.SetColor("_ShirtColor", targetColor);
      }
    }

    protected override void UpdateSpriteRenderer(int currentFrame, bool isMirrored) {
      for (int i = 0; i < layersBackToFront.Length; i++) {
        var spriteRenderer = _layerRenderers[i];
        spriteRenderer.sprite = layersBackToFront[i].frames[currentFrame];
        spriteRenderer.flipX = isMirrored;
      }
    }
  }
}