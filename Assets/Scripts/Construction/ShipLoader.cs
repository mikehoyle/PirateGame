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
    }

    private void Start() {
      InitializeScene();
    }
    
    private void InitializeScene() {
      foreach (var tileCoord in GameState.State.Player.Ship.Foundations) {
        _grid.Tilemap.SetTile(tileCoord, foundationTile);
      }
    }
  }
}