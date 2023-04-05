using System;
using System.Collections.Generic;
using Events;
using State.Unit;
using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class RangeIndicator : MonoBehaviour {
    [SerializeField] private TileBase eligibleTileOverlay;
    
    private Tilemap _tilemap;
    private SceneTerrain _terrain;

    public delegate bool ExcludeTile(Vector3Int tile);

    private void Awake() {
      _tilemap = GetComponent<Tilemap>();
      _terrain = SceneTerrain.Get();
    }

    private void OnEnable() {
      Dispatch.Encounters.PlayerTurnEnd.RegisterListener(OnPlayerTurnEnd);
    }

    private void OnDisable() {
      Dispatch.Encounters.PlayerTurnEnd.UnregisterListener(OnPlayerTurnEnd);
    }

    private void OnPlayerTurnEnd() {
      Clear();
      enabled = false;
    }

    public void DisplayMovementRange(Vector3Int unitPosition, int unitMoveRange, UnitFaction unitFaction) {
      enabled = true;
      Clear();
      foreach (var tile in _terrain.GetAllViableDestinations(unitPosition, unitMoveRange, unitFaction)) {
        _tilemap.SetTile(tile, eligibleTileOverlay);
      }
    }

    /// <summary>
    /// Displays targeting options, both min and max are inclusive
    /// TODO(P1): include line-of-sight.
    /// </summary>
    public void DisplayTargetingRange(Vector3Int source, int rangeMin, int rangeMax) {
      DisplayTargetingRangeWithExclusions(source, rangeMin, rangeMax, _ => false);
    }

    public void DisplayTargetingRangeWithExclusions(
        Vector3Int source, int rangeMin, int rangeMax, ExcludeTile excludeFunc) {
      enabled = true;
      Clear();
      // TODO(P2): Make a targeting-specific sprite
      _tilemap.color = Color.red;

      for (int x = -rangeMax; x <= rangeMax; x++) {
        var yRange = rangeMax - Math.Abs(x);
        for (int y = -yRange; y <= yRange; y++) {
          if (Math.Abs(x) + Math.Abs(y) < rangeMin) {
            continue;
          }
          var tile = _terrain.GetElevation(new Vector2Int(source.x + x, source.y + y));
          SetIfNotExcluded(tile, eligibleTileOverlay, excludeFunc);
        }
      }
    }

    public void DisplayStraightLineRange(
        Vector3Int source,
        int diagonalRangeMin,
        int diagonalRangeMax,
        int straightRangeMin,
        int straightRangeMax,
        ExcludeTile excludeFunc) {
      enabled = true;
      Clear();
      // TODO(P2): Make a targeting-specific sprite
      _tilemap.color = Color.red;

      if (diagonalRangeMax > 0) {
        for (int i = diagonalRangeMin; i <= diagonalRangeMax; i++) {
          SetIfNotExcluded(source + new Vector3Int(i, i, 0), eligibleTileOverlay, excludeFunc);
          SetIfNotExcluded(source + new Vector3Int(i, -i, 0), eligibleTileOverlay, excludeFunc);
          SetIfNotExcluded(source + new Vector3Int(-i, i, 0), eligibleTileOverlay, excludeFunc);
          SetIfNotExcluded(source + new Vector3Int(-i, -i, 0), eligibleTileOverlay, excludeFunc);
        }
      }

      if (straightRangeMax > 0) {
        for (int i = straightRangeMin; i <= straightRangeMax; i++) {
          SetIfNotExcluded(source + new Vector3Int(i, 0, 0), eligibleTileOverlay, excludeFunc);
          SetIfNotExcluded(source + new Vector3Int(0, i, 0), eligibleTileOverlay, excludeFunc);
          SetIfNotExcluded(source + new Vector3Int(-i, 0, 0), eligibleTileOverlay, excludeFunc);
          SetIfNotExcluded(source + new Vector3Int(0, -i, 0), eligibleTileOverlay, excludeFunc);
        }
      }
    }

    private void SetIfNotExcluded(Vector3Int coord, TileBase tile, ExcludeTile excludeFunc) {
      if (!excludeFunc(coord)) {
        _tilemap.SetTile(coord, tile);
      }
    }

    public void DisplayCustomRange(IEnumerable<Vector3Int> affectedTiles) {
      enabled = true;
      Clear();
      foreach (var tile in affectedTiles) {
        _tilemap.SetTile(tile, eligibleTileOverlay);
      }
    }

    public void Clear() {
      if (_tilemap != null) {
        _tilemap.ClearAllTiles();
        _tilemap.color = Color.white;
      }
    }
  }
}