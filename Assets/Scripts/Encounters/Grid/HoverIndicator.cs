using RuntimeVars.Encounters.Events;
using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class HoverIndicator : MonoBehaviour {
    [SerializeField] private EncounterEvents encounterEvents;
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
      encounterEvents.mouseHover.RegisterListener(OnMouseHover);
    }

    private void OnDisable() {
      encounterEvents.mouseHover.UnregisterListener(OnMouseHover);
    }

    private void OnMouseHover(Vector3Int hoveredTile) {
      _tilemap.ClearAllTiles();
      if (_terrain.GetTile((Vector2Int)hoveredTile) != null) {
        _tilemap.SetTile(hoveredTile, hoverIndicator);
      }
    }
  }
}