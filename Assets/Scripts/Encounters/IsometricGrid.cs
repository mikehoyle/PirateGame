using System.Collections.Generic;
using JetBrains.Annotations;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters {
  public class IsometricGrid : MonoBehaviour {
    private const float CellWidthInWorldUnits = 1;
    private const int MaxZ = 6;
  
    private const int MaxEncounterWidth = 300;
    private const int MaxEncounterHeight = 300;

    [SerializeField] private TileBase waterTile;
    
    private Camera _camera;
    
    public EncounterPathfindingGrid Pathfinder { get; private set; }

    public Grid Grid { get; private set; }
    public Tilemap Tilemap { get; private set; }
    public Tilemap Overlay { get; private set; }
    
    private void Awake() {
      _camera = Camera.main;
      Grid = GetComponent<Grid>();
      Tilemap = Grid.transform.Find("Tilemap").GetComponent<Tilemap>();
      Overlay = Grid.transform.Find("Overlay").GetComponent<Tilemap>();
      Pathfinder = new EncounterPathfindingGrid(MaxEncounterWidth, MaxEncounterHeight);
    }

    private void Start() {
      SetupPathfinding();
    }

    private void SetupPathfinding() {
      // OPTIMIZE: This is working in a pretty dumb way at the moment.
      //     it should probably work completely differently once I'm not working with prototype prefabs
      for (int x = -MaxEncounterWidth/2; x < MaxEncounterWidth; x++) {
        for (int y = -MaxEncounterHeight / 2; y < MaxEncounterHeight; y++) {
          var cell = GetTileAtPeakElevation(new Vector2Int(x, y));
          if (IsTileMovementEligible(cell)) {
            Pathfinder.MarkCellTraversable(cell);
          }
        }
      }
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

    // TODO(P2): For now, pathfinding completely ignores elevation
    [CanBeNull]
    public LinkedList<Vector3Int> GetPath(Vector3Int origin, Vector3Int destination) =>
        Pathfinder.GetPath(origin, destination);


    public static IsometricGrid Get() {
      return GameObject.FindWithTag(Tags.Grid).GetComponent<IsometricGrid>();
    }
  }
}