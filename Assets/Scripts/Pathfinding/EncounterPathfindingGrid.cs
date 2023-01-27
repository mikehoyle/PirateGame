using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Roy_T.AStar.Paths;
using UnityEngine;

namespace Pathfinding {
  public class EncounterPathfindingGrid {
    private static readonly List<Vector2Int> SurroundingNodes = new() {
        new(-1, 0),
        new(0, 1),
        new(1, 0),
        new(0, -1),
    };
    
    private readonly EncounterNode[,] _nodes;
    private readonly int _width;
    private readonly int _height;
    private readonly int _gridOffsetX;
    private readonly int _gridOffsetY;
    private readonly PathFinder _pathfinder;

    public EncounterPathfindingGrid(int width, int height, Func<Vector2Int, Vector3Int> getTileElevation) {
      _width = width;
      _height = height;
      _gridOffsetX = width / 2;
      _gridOffsetY = height / 2;
      _nodes = new EncounterNode[width, height];
      _pathfinder = new PathFinder();
      
      for (int i = 0; i < width; i++) {
        for (int j = 0; j < height; j++) {
          _nodes[i, j] = new EncounterNode(getTileElevation(new Vector2Int(i - _gridOffsetX, j - _gridOffsetY)));
        }
      }
    }
    
    private EncounterNode GetNode(Vector2Int coords) => _nodes[coords.x, coords.y];

    public void SetEnabled(Vector3Int coords, bool enabled) {
      GetNode(PositionForCoords(coords)).Enabled = enabled;
    }
    
    /// <summary>
    /// Returns proper elevation, but ignores elevation for pathing purposes.
    /// </summary>
    [CanBeNull]
    public LinkedList<Vector3Int> GetPath(Vector3Int origin, Vector3Int destination) {
      var originGridPosition = PositionForCoords(origin);
      var destinationGridPosition = PositionForCoords(destination);
      if (!IsGridPositionValid(originGridPosition) || !IsGridPositionValid(destinationGridPosition)) { 
        Debug.LogError("Attempt to access coordinate outside grid, returning invalid path.");
        return null;
      }
      
      var path = _pathfinder.FindPath(
          GetNode(originGridPosition), GetNode(destinationGridPosition), EncounterNode.TraversalVelocity);
      if (path.Type != PathType.Complete) {
        Debug.Log("Returning no path because no valid path was found");
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

      if (!IsGridPositionValid(gridPosition)) {
        Debug.LogError("Attempt to access coordinate outside grid, ignoring.");
        return;
      }

      var node = GetNode(gridPosition);
      
      foreach (var offset in SurroundingNodes) {
        var connectedNode = GetNode(gridPosition + offset);
        node.Connect(connectedNode);
      }
    }

    private Vector2Int PositionForCoords(Vector3Int coords) {
      // Just ignore Z value for now
      return new Vector2Int(coords.x + _gridOffsetX, coords.y + _gridOffsetY);
    }
    
    private bool IsGridPositionValid(Vector2Int gridPosition) {
      if (gridPosition.x < 0
          || gridPosition.y < 0
          || gridPosition.x > _width
          || gridPosition.y > _height) {
        return false;
      }
      return true;
    }
  }
}