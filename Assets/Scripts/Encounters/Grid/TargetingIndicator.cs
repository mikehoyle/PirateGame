using System;
using Common.Events;
using Events;
using Units.Abilities.AOE;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class TargetingIndicator : MonoBehaviour {
    [SerializeField] private TileBase targetedTileOverlay;
    
    private Tilemap _tilemap;

    private void Awake() {
      _tilemap = GetComponent<Tilemap>();
    }

    private void OnEnable() {
      Dispatch.Encounters.PlayerTurnEnd.RegisterListener(OnPlayerTurnEnd);
    }
    
    private void OnDisable() {
      Dispatch.Encounters.PlayerTurnEnd.UnregisterListener(OnPlayerTurnEnd);
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