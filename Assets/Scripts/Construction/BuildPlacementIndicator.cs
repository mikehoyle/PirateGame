using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Construction {
  /// <summary>
  /// Manages the display of not-yet-placed hints for placement, aka ghost builds.
  /// </summary>
  public class BuildPlacementIndicator : MonoBehaviour {
    [SerializeField] Color validPlacementColor;
    [SerializeField] Color invalidPlacementColor;
    
    private SpriteRenderer _spriteRenderer;
    private SceneTerrain _terrain;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _spriteRenderer.enabled = false;
    }

    public void ShowInvalidIndicator(Vector3Int gridPosition) {
      _spriteRenderer.color = invalidPlacementColor;
      ShowIndicator(gridPosition);
    }

    public void ShowValidIndicator(Vector3Int gridPosition) {
      _spriteRenderer.color = validPlacementColor;
      ShowIndicator(gridPosition);
    }

    public void SetSprite(Sprite sprite) {
      _spriteRenderer.sprite = sprite;
    }

    private void ShowIndicator(Vector3Int gridPosition) {
      transform.position = _terrain.CellAnchorWorld(gridPosition);
      _spriteRenderer.enabled = true;
    }

    public void Hide() {
      _spriteRenderer.enabled = false;
    }

  }
}