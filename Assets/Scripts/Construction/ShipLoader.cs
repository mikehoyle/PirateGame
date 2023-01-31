using System;
using CameraControl;
using Encounters;
using State;
using StaticConfig;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Construction {
  public class ShipLoader : MonoBehaviour {
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;
    
    private IsometricGrid _grid;

    private void Awake() {
      _grid = IsometricGrid.Get();
    }

    private void Start() {
      InitializeScene();
    }
    
    private void InitializeScene() {
      foreach (var build in GameState.State.Player.Ship.Components) {
        _grid.Tilemap.SetTile(build.Key, buildOptions.BuildMap[build.Value].inGameTile);
      }
    }
  }
}