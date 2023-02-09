﻿using System;
using Common.Events;
using Pathfinding;
using RuntimeVars.Encounters.Events;
using Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class RangeIndicator : MonoBehaviour {
    [SerializeField] private TileBase eligibleTileOverlay;
    [SerializeField] private EmptyGameEvent playerTurnEndEvent;
    
    private Tilemap _tilemap;
    private IsometricGrid _grid;
    private EncounterTerrain _terrain;

    private void Awake() {
      _tilemap = GetComponent<Tilemap>();
      _grid = IsometricGrid.Get();
      _terrain = EncounterTerrain.Get();
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

    public void DisplayMovementRange(UnitController unit) {
      enabled = true;
      var unitMoveRange = unit.State.encounterState.remainingMovement;
      var gridPosition = unit.State.encounterState.position;
      
      Clear();
      for (int x = -unitMoveRange; x <= unitMoveRange; x++) {
        var yMoveRange = unitMoveRange - Math.Abs(x);
        for (int y = -yMoveRange; y <= yMoveRange; y++) {
          if (x == 0 && y == 0) {
            continue;
          }
          var tile = _grid.GetTileAtPeakElevation(
              new Vector2Int(gridPosition.x + x, gridPosition.y + y));
          // OPTIMIZE: memoize paths
          var path = _terrain.GetPath(gridPosition, tile);
          if (path.IsViableAndWithinRange(unitMoveRange)) {
            _tilemap.SetTile(tile, eligibleTileOverlay);
          }
        }
      }
    }

    public void Clear() {
      _tilemap.ClearAllTiles();
      _tilemap.color = Color.white;
    }
  }
}