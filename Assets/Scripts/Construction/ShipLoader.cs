using System;
using CameraControl;
using Encounters;
using State;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Construction {
  public class ShipLoader : MonoBehaviour {
    [SerializeField] private TileBase foundationTile;
    
    private IsometricGrid _grid;
    private CameraController _camera;

    private void Awake() {
      _grid = IsometricGrid.Get();
      _camera = Camera.main.GetComponent<CameraController>();

      if (Debug.isDebugBuild) {
        // Expect game already loaded in prod build.
        GameState.Load();
      }
    }

    private void Start() {
      InitializeScene();
    }
    
    private void InitializeScene() {
      var minX = int.MaxValue;
      var maxX = int.MinValue;
      var minY = int.MaxValue;
      var maxY = int.MinValue;
      var shipState = GameState.State.Player.ShipState;
      foreach (var tileCoord in shipState.Foundations) {
        minX = Math.Min(minX, tileCoord.x);
        maxX = Math.Max(maxX, tileCoord.x);
        minY = Math.Min(minY, tileCoord.y);
        maxY = Math.Max(maxY, tileCoord.y);
        _grid.Tilemap.SetTile(tileCoord, foundationTile);
      }
      
      

      var visualMin = _grid.Grid.CellToWorld(new Vector3Int(minX, minY, 0));
      // +1 to maxes because CellToWorld returns bottom corner of cell,
      // so top corner of cell = bottom corner of caddy-cornered cell.
      var visualMax = _grid.Grid.CellToWorld(new Vector3Int(maxX + 1, maxY + 1, 0));
      
      _camera.SetFocusPoint(Vector3.Lerp(visualMin, visualMax, 0.5f));
    }
  }
}