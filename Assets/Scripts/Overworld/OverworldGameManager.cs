using State;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Overworld {
  public class OverworldGameManager : MonoBehaviour {
    [SerializeField] private TileBase _indicatorTile;
    private Tilemap _overlayTilemap;
    private Tilemap _overworldTilemap;

    private void Awake() {
      GameState.Load();
      _overworldTilemap = GameObject.Find("OverworldTilemap").GetComponent<Tilemap>();
      _overlayTilemap = GameObject.Find("OverlayTilemap").GetComponent<Tilemap>();
    }

    public void OnExitRequest() {
      Application.Quit(0);
    }

    public void OnConstructionMode() {
      // TODO load scene
    }
  }
}