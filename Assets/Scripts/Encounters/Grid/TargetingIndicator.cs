using System;
using Common.Events;
using Units.Abilities.AOE;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class TargetingIndicator : MonoBehaviour {
    [SerializeField] private TileBase targetedTileOverlay;
    [SerializeField] private EmptyGameEvent playerTurnEndEvent;
    
    private Tilemap _tilemap;

    private void Awake() {
      _tilemap = GetComponent<Tilemap>();
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

    public void TargetTiles(params Vector3Int[] tiles) {
      Clear();
      foreach (var tile in tiles) {
        _tilemap.SetTile(tile, targetedTileOverlay);
      }
    }

    public void TargetAoe(AreaOfEffect areaOfEffect) {
      Clear();
      foreach (var tile in areaOfEffect.AffectedPoints()) {
        _tilemap.SetTile(tile, targetedTileOverlay);
      }
    }

    public void Clear() {
      _tilemap.ClearAllTiles();
    }
  }
}