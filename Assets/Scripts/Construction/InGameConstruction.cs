using Common;
using Encounters;
using StaticConfig.Builds;
using Terrain;
using UnityEngine;

namespace Construction {
  public abstract class InGameConstruction : MonoBehaviour, IPlacedOnGrid {
    private SpriteRenderer _spriteRenderer;
    private SceneTerrain _terrain;

    public ConstructableObject Metadata { get; private set; }
    public Vector3Int Position { get; private set; }

    protected virtual void Awake() {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _terrain = SceneTerrain.Get();
    }

    public void Initialize(ConstructableObject constructableObject, Vector3Int position) {
      Metadata = constructableObject;
      _spriteRenderer.sprite = constructableObject.inGameSprite;
      Position = position;
      transform.position = constructableObject.isFoundationTile ?
          _terrain.CellAnchorWorld(position) : _terrain.CellBaseWorld(position);
      if (constructableObject.isFoundationTile) {
        _spriteRenderer.sortingLayerName = SortingLayers.Terrain;
      }
      InitializeInner(constructableObject, position);
    }

    protected virtual void InitializeInner(ConstructableObject constructableObject, Vector3Int position) { }
  }
}