using Common;
using Encounters;
using StaticConfig.Builds;
using UnityEngine;

namespace Construction {
  public abstract class InGameConstruction : MonoBehaviour, IPlacedOnGrid {
    private SpriteRenderer _spriteRenderer;
    private PolygonCollider2D _collider;

    public ConstructableObject Metadata { get; private set; }
    public Vector3Int Position { get; private set; }

    protected virtual void Awake() {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _collider = GetComponent<PolygonCollider2D>();
    }

    public void Initialize(ConstructableObject constructableObject, Vector3Int position, bool isGhost = false) {
      Metadata = constructableObject;
      _spriteRenderer.sprite = constructableObject.inGameSprite;
      if (isGhost) {
        _spriteRenderer.color = new Color(1, 1, 1, 0.5f); // Translucent
      }
      Position = position;
      transform.position = constructableObject.WorldPosition(position);
      _spriteRenderer.sortingLayerName = constructableObject.SortingLayer();
      ApplySize((Vector2Int)constructableObject.dimensions);
      InitializeInner(constructableObject, position);
    }

    private void ApplySize(Vector2Int size) {
      _collider.SetPath(0, GridUtils.GetFootprintPolygon(size));
    }

    protected virtual void InitializeInner(ConstructableObject constructableObject, Vector3Int position) { }
  }
}