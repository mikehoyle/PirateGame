using CameraControl;
using State;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Overworld {
  public class OverworldGameManager : MonoBehaviour {
    [SerializeField] private TileBase _indicatorTile;
    private Tilemap _overlayTilemap;
    private Vector3Int _knownPlayerPosition;
    private CameraController _camera;
    private Tilemap _overworldTilemap;

    private void Awake() {
      GameState.Load();
      _overworldTilemap = GameObject.Find("OverworldTilemap").GetComponent<Tilemap>();
      _overlayTilemap = GameObject.Find("OverlayTilemap").GetComponent<Tilemap>();
      _camera = Camera.main.GetComponent<CameraController>();
      _knownPlayerPosition = new Vector3Int(int.MinValue, int.MinValue);
    }

    private void Update() {
      if (_knownPlayerPosition.x != GameState.State.Player.OverworldGridPosition.x
          && _knownPlayerPosition.y != GameState.State.Player.OverworldGridPosition.y) {
        //_overlayTilemap.SetTile(_knownPlayerPosition, null);
        //_knownPlayerPosition = (Vector3Int)GameState.State.Player.OverworldGridPosition;
        //_overlayTilemap.SetTile(_knownPlayerPosition, _indicatorTile);

        _camera.SetFocusPoint(_overlayTilemap.CellToWorld(_knownPlayerPosition));
      }
    }
  }
}