using Encounters;
using StaticConfig.Builds;
using Terrain;
using UnityEngine;

namespace Construction {
  public class InGameConstruction : MonoBehaviour, IPlacedOnGrid {
    private SpriteRenderer _spriteRenderer;
    private SceneTerrain _terrain;

    public Vector3Int Position { get; private set; }

    private void Awake() {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _terrain = SceneTerrain.Get();
    }

    public void Initialize(ConstructableObject constructableObject, Vector3Int position) {
      _spriteRenderer.sprite = constructableObject.inGameSprite;
      Position = position;
      transform.position = _terrain.CellAnchorWorld(position);
    }
  }
}