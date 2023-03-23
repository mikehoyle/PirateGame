using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Grid;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Primitives;
using UnityEngine;

namespace Encounters.Generation {
  public class SimplePathfindingNode : INode {
    private Vector3Int _position;
    private readonly SimplePathfinder _pathfinder;

    public Position Position => new(_position.x, _position.y);
    public IList<IEdge> Incoming {
      get {
        var edges = new List<IEdge>();
        GridUtils.ForEachAdjacentTile(_position, adjacentTile => {
          if (_pathfinder.GetNode(adjacentTile).TryGet(out var node)) {
            edges.Add(new Edge(node, this, Velocity.FromMetersPerSecond(1)));
          }
        });
        return edges;
      }
    }
    public IList<IEdge> Outgoing {
      get {
        return Incoming.Select(edge => new Edge(edge.End, edge.Start, edge.TraversalVelocity)).Cast<IEdge>().ToList();
      }
    }

    public SimplePathfindingNode(Vector3Int position, SimplePathfinder pathfinder) {
      _position = position;
      _pathfinder = pathfinder;
    }
  }
}