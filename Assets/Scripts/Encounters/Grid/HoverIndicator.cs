using Events;
using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class HoverIndicator : MonoBehaviour {
    [SerializeField] private TileBase hoverIndicator;
    
    private SceneTerrain _terrain;
    private Tilemap _tilemap;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      if (_terrain == null) {
        enabled = false;
      }
      
      _tilemap = GetComponent<Tilemap>();
    }

    private void OnEnable() {
      Dispatch.Encounters.MouseHover.RegisterListener(OnMouseHover);
    }

    private void OnDisable() {
      Dispatch.Encounters.MouseHover.UnregisterListener(OnMouseHover);
    }

    private void OnMouseHover(Vector3Int hoveredTile) {
      _tilemap.ClearAllTiles();
      if (_terrain.GetTile((Vector2Int)hoveredTile) != null) {
        _tilemap.SetTile(hoveredTile, hoverIndicator);
      }
    }
  }
}