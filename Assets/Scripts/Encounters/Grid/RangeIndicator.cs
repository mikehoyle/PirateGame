﻿using System;
using System.Collections.Generic;
using Common.Events;
using Events;
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

    public void DisplayMovementRange(Vector3Int unitPosition, int unitMoveRange) {
      enabled = true;
      Clear();
      foreach (var tile in _terrain.GetAllViableDestinations(unitPosition, unitMoveRange)) {
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
          if (!excludeFunc(tile)) {
            _tilemap.SetTile(tile, eligibleTileOverlay);
          }
        }
      }
    }

    public void DisplayStraightLineRange(
        Vector3Int source, int diagonalRangeMin, int diagonalRangeMax, int straightRangeMin, int straightRangeMax) {
      enabled = true;
      Clear();
      // TODO(P2): Make a targeting-specific sprite
      _tilemap.color = Color.red;

      if (diagonalRangeMax > 0) {
        for (int i = diagonalRangeMin; i <= diagonalRangeMax; i++) {
          _tilemap.SetTile(source + new Vector3Int(i, i, 0), eligibleTileOverlay);
          _tilemap.SetTile(source + new Vector3Int(i, -i, 0), eligibleTileOverlay);
          _tilemap.SetTile(source + new Vector3Int(-i, i, 0), eligibleTileOverlay);
          _tilemap.SetTile(source + new Vector3Int(-i, -i, 0), eligibleTileOverlay);
        }
      }

      if (straightRangeMax > 0) {
        for (int i = straightRangeMin; i <= straightRangeMax; i++) {
          _tilemap.SetTile(source + new Vector3Int(i, 0, 0), eligibleTileOverlay);
          _tilemap.SetTile(source + new Vector3Int(0, i, 0), eligibleTileOverlay);
          _tilemap.SetTile(source + new Vector3Int(-i, 0, 0), eligibleTileOverlay);
          _tilemap.SetTile(source + new Vector3Int(0, -i, 0), eligibleTileOverlay);
        }
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
      _tilemap.ClearAllTiles();
      _tilemap.color = Color.white;
    }
  }
}