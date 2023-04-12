using System.Collections.Generic;
using UnityEngine;

namespace Common.Animation {
  public class CompositeDirectionalAnimator : DirectionalAnimator {
    private static readonly int ShirtColor = Shader.PropertyToID("_ShirtColor");
    private static readonly int HairColor = Shader.PropertyToID("_HairColor");
    private static readonly int PantsColor = Shader.PropertyToID("_PantsColor");
    private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");
    
    [SerializeField] private Material paletteSwapMaterial;
    [SerializeField] private DirectionalAnimatedSprite[] layersBackToFront;
    [SerializeField] private ColorCollection hairColorOptions;
    [SerializeField] private ColorCollection skinColorOptions;
    
    private List<SpriteRenderer> _layerRenderers;
    private List<DirectionalAnimatedSprite> _layerSprites;

    private void Awake() {
      AnimationTarget = GetComponent<IDirectionalAnimatable>();
      // We assume that all composite sprites have the same animations, so just use the first
      // (arbitrarily) as the reference for animations.
      referenceSprite = layersBackToFront[0];
      _layerRenderers = new();
      _layerSprites = new();
      foreach (var layer in layersBackToFront) {
        AddLayer(layer);
      }
    }

    // For now, random colors just to differentiate the units
    public void SetColor(string unitName) {
      // hacky way for consistent colors, just seed based on unit's name hash
      var rng = new System.Random(unitName.GetHashCode());
      var shirtColor = new Color(
          (float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble(), 1f);
      var pantsColor = new Color(
          (float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble(), 1f);
      var hairColor = hairColorOptions.colors[rng.Next(0, hairColorOptions.colors.Length)];
      var skinColor = skinColorOptions.colors[rng.Next(0, skinColorOptions.colors.Length)];
      foreach (var spriteRenderer in _layerRenderers) {
        spriteRenderer.material.SetColor(ShirtColor, shirtColor);
        spriteRenderer.material.SetColor(PantsColor, pantsColor);
        spriteRenderer.material.SetColor(HairColor, hairColor);
        spriteRenderer.material.SetColor(SkinColor, skinColor);
      }
    }

    public void AddLayer(DirectionalAnimatedSprite layer) {
      var layerRenderer = new GameObject("CompositeSpriteComponent") {
          layer = LayerMask.NameToLayer("UnitSprite"),
      };
      var spriteRenderer = layerRenderer.AddComponent<SpriteRenderer>();
      spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
      layerRenderer.transform.parent = transform;
      layerRenderer.transform.SetAsFirstSibling();
        
      // Set the z position slightly forward for each layer to cleanly sort.
      // Has no actual affect on positioning.
      var position = layerRenderer.transform.position;
      position.z = 0.0001f * _layerRenderers.Count;
      layerRenderer.transform.position = position;

      // Default to the first frame, for now.
      spriteRenderer.sprite = layer.frames[0];
      spriteRenderer.material = paletteSwapMaterial;
      _layerRenderers.Add(spriteRenderer);
      _layerSprites.Add(layer);
    }

    public void SetVisible(bool visible) {
      foreach (var layerRenderer in _layerRenderers) {
        layerRenderer.enabled = visible;
      }
    }

    protected override void UpdateSpriteRenderer(int currentFrame, bool isMirrored) {
      for (int i = 0; i < _layerSprites.Count; i++) {
        var spriteRenderer = _layerRenderers[i];
        spriteRenderer.sprite = _layerSprites[i].frames[currentFrame];
        spriteRenderer.flipX = isMirrored;
      }
    }
  }
}