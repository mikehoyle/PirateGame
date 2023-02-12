﻿using System;
using Common.Events;
using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class RangeIndicator : MonoBehaviour {
    [SerializeField] private TileBase eligibleTileOverlay;
    [SerializeField] private EmptyGameEvent playerTurnEndEvent;
    
    private Tilemap _tilemap;
    private SceneTerrain _terrain;

    private void Awake() {
      _tilemap = GetComponent<Tilemap>();
      _terrain = SceneTerrain.Get();
    }

    private void OnEnable() {
      playerTurnEndEvent.RegisterListener(OnPlayerTurnEnd);
    }

    private void OnDisable() {
      playerTurnEndEvent.RegisterListener(OnPlayerTurnEnd);
    }

    private void OnPlayerTurnEnd() {
      enabled = false;
    }

    public void DisplayMovementRange(Vector3Int unitPosition, int unitMoveRange) {
      enabled = true;
      Clear();
      foreach (var tile in _terrain.GetAllViableDestinations(unitPosition, unitMoveRange)) {
        if (tile.x == 0 && tile.y == 0) {
          continue;
        }
        _tilemap.SetTile(tile, eligibleTileOverlay);
      }
    }

    /// <summary>
    /// Displays targeting options, both min and max are inclusive
    /// TODO(P1): include line-of-sight.
    /// </summary>
    public void DisplayTargetingRange(Vector3Int source, int rangeMin, int rangeMax) {
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
          _tilemap.SetTile(tile, eligibleTileOverlay);
        }
      }

    }

    public void Clear() {
      _tilemap.ClearAllTiles();
      _tilemap.color = Color.white;
    }
  }
}