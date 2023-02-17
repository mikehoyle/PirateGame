using System;
using Common;
using RuntimeVars.ShipBuilder;
using RuntimeVars.ShipBuilder.Events;
using StaticConfig.Builds;
using Terrain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Construction {
  /// <summary>
  /// Manages the display of not-yet-placed hints for placement, aka ghost builds.
  /// </summary>
  public class BuildPlacementIndicator : MonoBehaviour {
    [SerializeField] Color validPlacementColor;
    [SerializeField] Color invalidPlacementColor;
    [SerializeField] private CurrentBuildSelection currentBuildSelection;
    
    private SpriteRenderer _spriteRenderer;
    private SceneTerrain _terrain;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _spriteRenderer.enabled = false;
    }

    private void Update() {
      if (!currentBuildSelection.build.TryGet(out var build) || !currentBuildSelection.tile.TryGet(out var tile)) {
        Hide();
        return;
      }
      
      SetSprite(build.inGameSprite);
      if (currentBuildSelection.isValidPlacement) {
        _spriteRenderer.color = validPlacementColor;
        ShowIndicator(tile, build);
      } else {
        _spriteRenderer.color = invalidPlacementColor;
        ShowIndicator(tile, build);
      }
    }

    private void SetSprite(Sprite sprite) {
      _spriteRenderer.sprite = sprite;
    }

    private void ShowIndicator(Vector3Int gridPosition, ConstructableObject build) {
      transform.position = build.isFoundationTile ?
          _terrain.CellAnchorWorld(gridPosition) : _terrain.CellBaseWorld(gridPosition);

      _spriteRenderer.enabled = true;
    }

    private void Hide() {
      _spriteRenderer.enabled = false;
    }
  }
}