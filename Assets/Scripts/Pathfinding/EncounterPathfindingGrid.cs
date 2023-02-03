using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using Roy_T.AStar.Paths;
using UnityEngine;

namespace Pathfinding {
  public class EncounterPathfindingGrid : MonoBehaviour {
    private const int MaxEncounterWidth = 300;
    private const int MaxEncounterHeight = 300;
    
    private EncounterNode[,] _nodes;
    private int _width;
    private int _height;
    private int _gridOffsetX;
    private int _gridOffsetY;
    private PathFinder _pathfinder;

    private void Awake() {
      _width = MaxEncounterWidth;
      _height = MaxEncounterHeight;
      _gridOffsetX = MaxEncounterWidth / 2;
      _gridOffsetY = MaxEncounterHeight / 2;
      _nodes = new EncounterNode[MaxEncounterWidth, MaxEncounterHeight];
      _pathfinder = new PathFinder();
    }
    
    private EncounterNode GetNode(Vector2Int coords) => _nodes[coords.x, coords.y];

    public void SetEnabled(Vector3Int coords, bool enabled) {
      if (!IsGridPositionDefined(PositionForCoords(coords))) {
        return;
      }
      GetNode(PositionForCoords(coords)).Enabled = enabled;
    }
    
    /// <summary>
    /// Returns proper elevation, but ignores elevation for pathing purposes.
    /// </summary>
    [CanBeNull]
    public LinkedList<Vector3Int> GetPath(Vector3Int origin, Vector3Int destination) {
      var originGridPosition = PositionForCoords(origin);
      var destinationGridPosition = PositionForCoords(destination);
      if (!IsGridPositionDefined(originGridPosition) || !IsGridPositionDefined(destinationGridPosition)) { 
        //Debug.Log("Attempt to access coordinate outside grid, returning invalid path.");
        return null;
      }
      
      var path = _pathfinder.FindPath(
          GetNode(originGridPosition), GetNode(destinationGridPosition), EncounterNode.TraversalVelocity);
      if (path.Type != PathType.Complete) {
        return null;
      }

      var result = new LinkedList<Vector3Int>();
      if (path.Edges.Count > 0) {
        result.AddLast(((EncounterNode)path.Edges[0].Start).GridPosition);
      }
      foreach (var edge in path.Edges) {
        result.AddLast(((EncounterNode)edge.End).GridPosition);
      }
      return result;
    }

    public void MarkCellTraversable(Vector3Int cell) {
      var gridPosition = PositionForCoords(cell);

      if (!IsGridPositionInBounds(gridPosition)) {
        Debug.LogWarning("Attempt to access coordinate outside grid, ignoring.");
        return;
      }

      var node = GetNode(gridPosition);
      if (node == null) {
        node = new EncounterNode(cell);
        _nodes[gridPosition.x, gridPosition.y] = node;
      }
      
      IsometricGridUtils.ForEachAdjacentTile(gridPosition, adjacentPosition => {
        var connectedNode = GetNode(adjacentPosition);
        if (connectedNode != null) {
          node.Connect(connectedNode);
        }
      });
    }

    private Vector2Int PositionForCoords(Vector3Int coords) {
      // Just ignore Z value for now
      return new Vector2Int(coords.x + _gridOffsetX, coords.y + _gridOffsetY);
    }
    
    private bool IsGridPositionDefined(Vector2Int gridPosition) {
      if (!IsGridPositionInBounds(gridPosition) || GetNode(gridPosition) == null) {
        return false;
      }
      
      return true;
    }

    private bool IsGridPositionInBounds(Vector2Int gridPosition) {
      return gridPosition.x >= 0
          && gridPosition.y >= 0
          && gridPosition.x <= _width
          && gridPosition.y <= _height;
    }

    public static EncounterPathfindingGrid Get() {
      return GameObject.FindWithTag(Tags.Grid).GetComponent<EncounterPathfindingGrid>();
    }
  }
}