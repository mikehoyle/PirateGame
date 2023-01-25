using State;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Overworld {
  public class OverworldGameManager : MonoBehaviour {
    [SerializeField] private TileBase _indicatorTile;
    private Tilemap _overlayTilemap;
    private Vector3Int _knownPlayerPosition;
    private Camera _camera;

    private void Awake() {
      GameState.Load();
      _overlayTilemap = GameObject.Find("OverlayTilemap").GetComponent<Tilemap>();
      _camera = Camera.main;
      _knownPlayerPosition = new Vector3Int(int.MinValue, int.MinValue);
    }

    private void Update() {
      if (_knownPlayerPosition.x != GameState.State.Player.OverworldPosition.x
          && _knownPlayerPosition.y != GameState.State.Player.OverworldPosition.y) {
        _overlayTilemap.SetTile(_knownPlayerPosition, null);
        _knownPlayerPosition = (Vector3Int)GameState.State.Player.OverworldPosition;
        _overlayTilemap.SetTile(_knownPlayerPosition, _indicatorTile);

        var worldPosition = _overlayTilemap.CellToWorld(_knownPlayerPosition);
        _camera.transform.position = new Vector3(worldPosition.x, worldPosition.y, -10);
      }
    }
  }
}