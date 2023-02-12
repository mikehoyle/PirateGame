using System.Collections.Generic;
using Roy_T.AStar.Paths;
using UnityEngine;

namespace Terrain {
  public class TerrainPathfinder {
    private readonly PathFinder _pathfinder;
    
    public TerrainPathfinder() {
      _pathfinder = new PathFinder();
    }
    
    public TravelPath GetPath(TerrainTile originNode, TerrainTile destinationNode) {
      var path = _pathfinder.FindPath(originNode, destinationNode, TerrainTile.TraversalVelocity);
      if (path.Type != PathType.Complete) {
        return new TravelPath(null);
      }

      var result = new LinkedList<Vector3Int>();
      if (path.Edges.Count > 0) {
        result.AddLast(((TerrainTile)path.Edges[0].Start).GridPosition);
      }
      foreach (var edge in path.Edges) {
        result.AddLast(((TerrainTile)edge.End).GridPosition);
      }
      return new TravelPath(result);
    }
  }
}