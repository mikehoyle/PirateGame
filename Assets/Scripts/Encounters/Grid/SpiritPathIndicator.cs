using System;
using RuntimeVars.Encounters;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class SpiritPathIndicator : MonoBehaviour {
    [SerializeField] private TileBase spiritPathTile;
    [SerializeField] private SpiritCollection spiritsInEncounter;
    
    private Tilemap _tilemap;
    
    private void Awake() {
      _tilemap = GetComponent<Tilemap>();
    }

    private void Update() {
      _tilemap.ClearAllTiles();
      foreach (var spirit in spiritsInEncounter.spirits) {
        var path = spirit.GetPath();
        foreach (var tile in path) {
          _tilemap.SetTile(tile, spiritPathTile);
        }
      }
    }
  }
}