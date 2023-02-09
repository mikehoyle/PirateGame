using System;
using Common.Events;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class TargetingIndicator : MonoBehaviour {
    [SerializeField] private TileBase targetedTileOverlay;
    [SerializeField] private EmptyGameEvent playerTurnEndEvent;
    
    private Tilemap _tilemap;
    private IsometricGrid _grid;

    private void Awake() {
      _tilemap = GetComponent<Tilemap>();
      _grid = IsometricGrid.Get();
    }

    private void OnEnable() {
      playerTurnEndEvent.RegisterListener(OnPlayerTurnEnd);
    }
    
    private void OnDisable() {
      playerTurnEndEvent.UnregisterListener(OnPlayerTurnEnd);
    }

    private void OnPlayerTurnEnd() {
      enabled = false;
    }

    public void TargetTile(Vector3Int tile) {
      Clear();
      _tilemap.SetTile(tile, targetedTileOverlay);
    }

    public void Clear() {
      _tilemap.ClearAllTiles();
    }
  }
}