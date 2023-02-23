using System;
using System.Collections.Generic;
using Common;
using Encounters;
using StaticConfig.Terrain;
using UnityEngine;

namespace Terrain {
  /// <summary>
  /// Represents the walkable terrain of the scene.
  /// </summary>
  public class SceneTerrain : MonoBehaviour {
    public const float CellWidthInWorldUnits = 1;
    public const float CellHeightInWorldUnits = 0.5f;
    private const float ZHeight = 1.5f;
    private const int MaxZ = 6;
    
    // TODO(P2): Use custom tilemaps instead of passed in sprites
    [SerializeField] private TerrainTilemap tilemap;
    [SerializeField] private GameObject terrainTilePrefab;

    
    private SparseMatrix3d<TerrainTile> _terrainMap;
    private TerrainPathfinder _pathfinder;
    private Camera _camera;
    
    public Grid Grid { get; private set; }

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
      GridUtils.ForEachAdjacentTile(position, adjacentCoords => {
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
      if (!IsGridPositionDefined(origin) || !IsGridPositionDefined(destination)) {
        return new TravelPath(null);
      }

      var originNode = _terrainMap[origin];
      var originNodeEnabledStatus = originNode.Enabled;
      originNode.Enabled = true;
      var result = _pathfinder.GetPath(originNode, _terrainMap[destination]);
      originNode.Enabled = originNodeEnabledStatus;
      return result;
    }

    public List<Vector3Int> GetAllViableDestinations(Vector3Int position, int moveRange) {
      var result = new List<Vector3Int>();
      for (int x = -moveRange; x <= moveRange; x++) {
        var yMoveRange = moveRange - Math.Abs(x);
        for (int y = -yMoveRange; y <= yMoveRange; y++) {
          // OPTIMIZE: memoize paths
          var path = GetPath(position, new Vector3Int(position.x + x, position.y + y));
          if (path.IsViableAndWithinRange(moveRange)) {
            result.Add(path.Path.Last.Value);
          }
        }
      }

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

    /// <summary>
    /// Because this is just a simple calculation, there's no reason we can't calculate it on our own
    /// without needing access to the game object 
    /// </summary>
    public static Vector3 CellBaseWorldStatic(Vector3Int coord) {
      return new Vector3(
          (coord.x - coord.y) * (CellWidthInWorldUnits / 2),
          ((coord.x + coord.y) * (CellHeightInWorldUnits / 2))
          + (coord.z * ZHeight * (CellHeightInWorldUnits / 2)),
          coord.z * ZHeight);
    }

    public static Vector3 CellCenterWorldStatic(Vector3Int coord) {
      return CellBaseWorldStatic(coord) + new Vector3(0, CellHeightInWorldUnits / 2, 0);
    }
    
    public static Vector3 CellAnchorWorldStatic(Vector3Int coord) {
      return CellBaseWorldStatic(coord) + new Vector3(0, CellHeightInWorldUnits, 0);
    }

    public static bool IsMovementBlocked(Vector3Int tile) {
      var blockingLayer = LayerMask.GetMask("BlockMovement");
      foreach (var collision in Physics2D.OverlapPointAll(CellCenterWorldStatic(tile), blockingLayer)) {
        // A simple check isn't sufficient, because isometric elevation can overlay elevated tiles on top
        // of lower tiles behind them, so check the Z position of collisions, if possible
        var placedOnGrid = collision.GetComponentInParent<IPlacedOnGrid>(); 
        if (placedOnGrid != null) {
          if (placedOnGrid.Position.z == tile.z) {
            return true;
          }
        } else {
          // This should hopefully not be the case, but worst case assume an intersection is blocking
          return true;
        }
      }

      return false;
    }

    public static GameObject GetTileOccupant(Vector3Int tile) {
      var blockingLayer = LayerMask.GetMask("BlockMovement");
      // Because movement blockers are on their own layer, they must be children to the primary object. This makes
      // the bold assumption they will always be direct children. Surely will cause a bug in the future, but it's the
      // easiest way for now.
      return Physics2D.OverlapPoint(CellCenterWorldStatic(tile), blockingLayer)?.gameObject;
    }

    public bool IsTileEligibleForUnitOccupation(Vector3Int gridPosition) {
      return IsGridPositionDefined(gridPosition) && !IsMovementBlocked(gridPosition);
    }

    private bool IsGridPositionDefined(Vector3Int gridPosition) {
      return _terrainMap.Contains(gridPosition);
    }

    public static SceneTerrain Get() {
      return GameObject.FindWithTag(Tags.Terrain).GetComponent<SceneTerrain>();
    }
  }
}