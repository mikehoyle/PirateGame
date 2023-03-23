using System.Collections.Generic;
using Optional;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using UnityEngine;

namespace Encounters.Generation {
  public class SimplePathfinder {
    private readonly Dictionary<Vector3Int, SimplePathfindingNode> _terrain;
    private readonly PathFinder _pathfinder;
    public List<Vector3Int> BlockedTiles { get; } = new();

    public SimplePathfinder(IEnumerable<Vector3Int> terrain) {
      _pathfinder = new PathFinder();
      _terrain = new();
      foreach (var tile in terrain) {
        _terrain.Add(tile, new SimplePathfindingNode(tile, this));
      }
    }

    public Option<SimplePathfindingNode> GetNode(Vector3Int position) {
      if (BlockedTiles.Contains(position) || !_terrain.TryGetValue(position, out var node)) {
        return Option.None<SimplePathfindingNode>();
      }

      return Option.Some(node);
    }

    public bool IsPathAccessible(Vector3Int from, Vector3Int to) {
      if (!_terrain.TryGetValue(from, out var fromNode) || !_terrain.TryGetValue(to, out var toNode)) {
        return false;
      }

      if (from == to) {
        return true;
      }

      var pathResult = _pathfinder.FindPath(fromNode, toNode, Velocity.FromMetersPerSecond(1));
      return pathResult.Type == PathType.Complete;
    }
  }
}