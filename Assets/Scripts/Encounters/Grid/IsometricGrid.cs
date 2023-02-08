using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters.Grid {
  public class IsometricGrid : MonoBehaviour {
    private const float CellWidthInWorldUnits = 1;
    private const int MaxZ = 6;

    [SerializeField] private TileBase waterTile;
    
    private Camera _camera;

    public UnityEngine.Grid Grid { get; private set; }
    public Tilemap Tilemap { get; private set; }
    
    private void Awake() {
      _camera = Camera.main;
      Grid = GetComponent<UnityEngine.Grid>();
      Tilemap = Grid.transform.Find("Tilemap").GetComponent<Tilemap>();
    }

    /// <summary>
    /// Because we support elevation via the Z coordinate, WorldToCell will not work out of the
    /// box. To accomodate, we loop through the possible Z values from above and adjust the world
    /// position by their display offset from the grid, and check if that tile exists. If not, keep searching.
    /// </summary>
    public Vector3Int TileAtScreenCoordinate(Vector2 screenCoord) {
      var worldPoint = _camera.ScreenToWorldPoint(screenCoord);
      // Get rid of the default camera-level elevation.
      worldPoint.z = 0;
      var adjustmentPerZPx = new Vector3(0, CellWidthInWorldUnits / 4f, 0);
    
      for (int z = MaxZ; z >= 0; z--) {
        var adjustedWorldPoint = worldPoint - (z * adjustmentPerZPx);
        var gridCell = Grid.WorldToCell(adjustedWorldPoint);
        gridCell.z = z;
        if (Tilemap.HasTile(gridCell) || z == 0) {
          return gridCell;
        }
      }

      var fallback = Grid.WorldToCell(worldPoint);
      fallback.z = 0;
      return fallback;
    }

    public Vector3Int GetTileAtPeakElevation(Vector2Int position) {
      Vector3Int cell = new Vector3Int(position.x, position.y, 0);
      for (int z = MaxZ; z >= 0; z--) {
        cell = new Vector3Int(position.x, position.y, z);
        if (Tilemap.HasTile(cell)) {
          break;
        }
      }
    
      return cell;
    }

    public bool IsTileMovementEligible(Vector3Int position) {
      // OPTIMIZE: This could be optimized surely, and will need to change
      var tile = Tilemap.GetTile(position);
      return tile != null && tile != waterTile;
    }


    public static IsometricGrid Get() {
      return GameObject.FindWithTag(Tags.Grid).GetComponent<IsometricGrid>();
    }
  }
}