using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Grid;
using Encounters;
using StaticConfig.Terrain;
using UnityEngine;
using static Common.Grid.GridUtils;

namespace Terrain {
  /// <summary>
  /// Represents the walkable terrain of the scene.
  /// </summary>
  public class SceneTerrain : MonoBehaviour {
    private const int MaxZ = 6;
    
    // TODO(P2): Use custom tilemaps instead of passed in sprites
    [SerializeField] private TerrainTilemap tilemap;
    [SerializeField] private GameObject terrainTilePrefab;

    
    private SparseMatrix3d<TerrainTile> _terrainMap;
    private TerrainPathfinder _pathfinder;
    private Camera _camera;
    
    public Grid Grid { get; private set; }
    public ICollection<Vector3Int> AllTiles => _terrainMap.Keys;

    private void Awake() {
      Grid = GetComponent<Grid>();
      _terrainMap = new SparseMatrix3d<TerrainTile>();
      _pathfinder = new TerrainPathfinder();
      _camera = Camera.main;
    }

    public void AddTerrain(Vector3Int position, Sprite terrainSprite) {
      // Only one tile per elevation, remove any that break that.
      RemoveTilesAtAnyElevation(position);

      var newTile = Instantiate(terrainTilePrefab, transform).GetComponent<TerrainTile>();
      newTile.Initialize(position, terrainSprite);
      
      // By default, connect tiles to any adjacent tile that is within one Z value of
      ForEachAdjacentTile(position, adjacentCoords => {
        var adjacentTile = GetTile((Vector2Int)adjacentCoords);
        if (adjacentTile == null) {
          return;
        }
        if (Math.Abs(adjacentTile.GridPosition.z - position.z) <= 1) {
          newTile.Connect(adjacentTile);
        }
      });

      _terrainMap[position] = newTile;
    }

    private void RemoveTilesAtAnyElevation(Vector3Int position) {
      for (int z = 0; z <= MaxZ; z++) {
        var currentPosition = new Vector3Int(position.x, position.y, z);
        if (_terrainMap.TryGetValue(currentPosition, out var tile)) {
          tile.Remove();
          _terrainMap.Remove(currentPosition);
        }
      }
    }

    /// <summary>
    /// Gets a tile at the given position, any elevation.
    /// </summary>
    public TerrainTile GetTile(Vector2Int position) {
      for (int z = 0; z <= MaxZ; z++) {
        var currentPosition = new Vector3Int(position.x, position.y, z);
        if (_terrainMap.TryGetValue(currentPosition, out var tile)) {
          return tile;
        }
      }
      return null;
    }

    public Vector3Int GetElevation(Vector2Int position) {
      return GetTile(position)?.GridPosition ?? new Vector3Int(position.x, position.y, 0);
    }

    /// <summary>
    /// Returns proper elevation, but ignores elevation for pathing purposes.
    /// Always ignores enabled status of origin tile, to ensure units never block themselves.
    /// </summary>
    public TravelPath GetPath(Vector3Int origin, Vector3Int destination) {
      return GetPath(origin, destination, new SparseMatrix3d<bool>());
    }
    
    public TravelPath GetPath(Vector3Int origin, Vector3Int destination, SparseMatrix3d<bool> enablementOverrides) {
      if (!IsGridPositionDefined(origin) || !IsGridPositionDefined(destination)) {
        return new TravelPath(null);
      }

      if (IsTileOccupied(destination)) {
        return new TravelPath(null);
      }
      
      // Always set origin to enabled, otherwise the actor is locked out by themselves.
      enablementOverrides[origin] = true;
      var originalEnablements = new SparseMatrix3d<bool>();
      foreach (var enablementOverride in enablementOverrides) {
        if (_terrainMap.TryGetValue(enablementOverride.Key, out var terrainTile)) {
          originalEnablements[enablementOverride.Key] = terrainTile.Enabled;
          terrainTile.Enabled = enablementOverride.Value;
        }
      }

      var result = _pathfinder.GetPath(_terrainMap[origin], _terrainMap[destination]);
      
      foreach (var originalEnablement in originalEnablements) {
        if (_terrainMap.TryGetValue(originalEnablement.Key, out var terrainTile)) {
          terrainTile.Enabled = originalEnablements[originalEnablement.Key];
        }
      }
      return result;
    }

    public HashSet<Vector3Int> GetAllViableDestinations(Vector3Int position, int moveRange) {
      return GetAllViableDestinations(position, moveRange, new());
    }

    public HashSet<Vector3Int> GetAllViableDestinations(
        Vector3Int position, int moveRange, SparseMatrix3d<bool> enablementOverrides) {
      var result = new HashSet<Vector3Int>();
      for (int x = -moveRange; x <= moveRange; x++) {
        var yMoveRange = moveRange - Math.Abs(x);
        for (int y = -yMoveRange; y <= yMoveRange; y++) {
          var possibleDestination = new Vector3Int(position.x + x, position.y + y);
          if (result.Contains(possibleDestination)) {
            // OPTIMIZE: memoize paths better
            continue;
          }
          
          var path = GetPath(position, possibleDestination, enablementOverrides);
          if (path.IsViableAndWithinRange(moveRange)) {
            foreach (var node in path.Path) {
              if (!IsTileOccupied(node)) {
                result.Add(node);
              }
            }
          }
        }
      }

      result.Remove(position);
      return result;
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
        if (IsGridPositionDefined(gridCell) || z == 0) {
          return gridCell;
        }
      }

      var fallback = Grid.WorldToCell(worldPoint);
      fallback.z = 0;
      return fallback;
    }

    /// <summary>
    /// We have to adjust the anchor, because (bafflingly) Unity wants to sort tiles based on their bottom-center
    /// corner, which will result in all sorts of clipping issues. This adjustment places them with the assumption
    /// that their pivot is at top-center (where we place it), which resolves all sorts of woes.
    /// </summary>
    public Vector3 CellAnchorWorld(Vector3Int coord) {
       return Grid.CellToWorld(coord + new Vector3Int(1, 1, 0));
    }
    
    public Vector3 CellBaseWorld(Vector3Int coord) {
      return Grid.CellToWorld(coord);
    }

    /// <summary>
    /// Grid.CellCenterWorld seems broken, so this replaces it.
    /// </summary>
    public Vector3 CellCenterWorld(Vector3Int coord) {
      return Grid.CellToWorld(coord) + new Vector3(0, Grid.cellSize.y / 2, 0);
    }

    public static bool IsMovementBlocked(Vector3Int tile) {
      var objectLayer = LayerMask.GetMask("PlacedOnGrid");
      foreach (var collision in Physics2D.OverlapPointAll(GridUtils.CellCenterWorld(tile), objectLayer)) {
        if (collision.TryGetComponent<IPlacedOnGrid>(out var placedOnGrid)) {
          // Committing to not doing elevation right here right now. It would be necessary to check here.
          return placedOnGrid.BlocksAllMovement;
        }
      }
      return false;
    }
    
    public static bool IsTileOccupied(Vector3Int tile) {
      var objectLayer = LayerMask.GetMask("PlacedOnGrid");
      foreach (var collision in Physics2D.OverlapPointAll(GridUtils.CellCenterWorld(tile), objectLayer)) {
        if (collision.TryGetComponent<IPlacedOnGrid>(out var placedOnGrid)) {
          return placedOnGrid.ClaimsTile;
        }
      }
      return false;
    }

    public RectInt GetBoundingRect() {
      return _terrainMap.GetBoundingRect();
    }

    public static GameObject GetTileOccupant(Vector3Int tile) {
      var allAtTile = GetAllTileOccupants(tile);
      if (allAtTile.Count == 0) {
        return null;
      }
      if (allAtTile.Count == 1) {
        return allAtTile[0];
      }
      
      // This is quick an dirty but handles stacking on bones for now.
      foreach (var occupant in allAtTile) {
        if (occupant.TryGetComponent<IPlacedOnGrid>(out var gridOccupant)) {
          if (gridOccupant.ClaimsTile) {
            return occupant;
          }
        }
      }
      return allAtTile[0];
    }

    public static List<GameObject> GetAllTileOccupants(Vector3Int tile) {
      var blockingLayer = LayerMask.GetMask("PlacedOnGrid");
      var allAtTile = Physics2D.OverlapPointAll(GridUtils.CellCenterWorld(tile), blockingLayer);
      return allAtTile.Select(c => c.gameObject).ToList();
    }

    public static bool TryGetComponentAtTile<T>(Vector3Int tile, out T component) {
      foreach (var occupant in GetAllTileOccupants(tile)) {
        if (occupant.TryGetComponent(out component)) {
          return true;
        }
      }
      component = default(T);
      return false;
    }

    public static bool TryGetTile(Vector3Int coord, out TerrainTile tile) {
      var blockingLayer = LayerMask.GetMask("Terrain");
      var terrainAtTile = Physics2D.OverlapPoint(GridUtils.CellCenterWorld(coord), blockingLayer);
      if (terrainAtTile != null && terrainAtTile.TryGetComponent(out tile)) {
        return true;
      }

      tile = null;
      return false;
    }

    public bool IsTileEligibleForUnitOccupation(Vector3Int gridPosition) {
      return IsGridPositionDefined(gridPosition) && !IsTileOccupied(gridPosition);
    }

    private bool IsGridPositionDefined(Vector3Int gridPosition) {
      return _terrainMap.Contains(gridPosition);
    }
    
    public Vector3Int GetRandomAvailableTile(Vector2Int size, Func<Vector3Int, bool> constraint) {
      var rng = new System.Random();
      using var randomizedTiles = _terrainMap.Keys.Where(constraint).OrderBy(_ => rng.Next()).GetEnumerator();
      while (randomizedTiles.MoveNext()) {
        var tile = randomizedTiles.Current;
        if (AllTilesAreFreeForSize(tile, size)) {
          return tile;
        }
      }
      
      Debug.LogError("Could not find tile to claim, this shouldn't happen!");
      return new Vector3Int(99999, 99999, 0);
    }
    
    public bool AllTilesAreFreeForSize(Vector3Int tile, Vector2Int size) {
      for (int x = 0; x < size.x; x++) {
        for (int y = 0; y < size.y; y++) {
          if (!IsTileEligibleForUnitOccupation(new Vector3Int(tile.x + x, tile.y + y, tile.z))) {
            return false;
          }
        }
      }
      
      return true;
    }

    public static SceneTerrain Get() {
      return GameObject.FindWithTag(Tags.Terrain).GetComponent<SceneTerrain>();
    }
  }
}