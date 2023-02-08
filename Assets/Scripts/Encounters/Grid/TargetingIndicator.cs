using System;
using Common.Events;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class TargetingIndicator : MonoBehaviour {
    [SerializeField] private TileBase eligibleTileOverlay;
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

    // TODO(P0): Finish this
    public void DisplayAttackPossibilities(Vector3Int gridPosition) {
      enabled = true;
      Clear();
      _tilemap.color = Color.red;

      for (int x = -1; x <= 1; x++) {
        var yRange = 1 - Math.Abs(x);
        for (int y = -yRange; y <= yRange; y++) {
          if (x == 0 && y == 0) {
            continue;
          }
          var tile = _grid.GetTileAtPeakElevation(
              new Vector2Int(gridPosition.x + x, gridPosition.y + y));
          if (_grid.IsTileMovementEligible(tile)) {
            _tilemap.SetTile(tile, eligibleTileOverlay);
          }
        }
      }
    }

    public void Clear() {
      _tilemap.ClearAllTiles();
    }
  }
}