using UnityEngine;
using UnityEngine.Tilemaps;

namespace Construction {
  /// <summary>
  /// Manages the display of not-yet-placed hints for placement, aka ghost builds.
  /// </summary>
  public class BuildPlacementIndicator : MonoBehaviour {
    [SerializeField] Color validPlacementColor;
    [SerializeField] Color invalidPlacementColor;
    
    private Grid _grid;
    private SpriteRenderer _spriteRenderer;

    private void Awake() {
      _grid = GetComponentInParent<Grid>();
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

    private void ShowIndicator(Vector3Int gridPosition) {
      // x+1, y+1 accommodates for top-right tile anchor.
      transform.position = _grid.CellToWorld(gridPosition + new Vector3Int(1, 1, 0));
      _spriteRenderer.enabled = true;
    }

    public void Hide() {
      _spriteRenderer.enabled = false;
    }

  }
}