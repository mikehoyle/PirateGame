using System.Collections.Generic;
using System.Linq;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Primitives;
using UnityEngine;

namespace Pathfinding {
  public class EncounterNode : INode {
    public static readonly Velocity TraversalVelocity = Velocity.FromMetersPerSecond(1);

    private readonly IList<IEdge> _incoming;
    private readonly IList<IEdge> _outgoing;
    private readonly IList<IEdge> _emptyList;

    public Vector3Int GridPosition { get; }

    public Position Position => new(GridPosition.x, GridPosition.y);
    
    public IList<IEdge> Incoming {
      get {
        if (Enabled) {
          return _incoming.Where(edge => ((EncounterNode)edge.Start).Enabled).ToList();
        }
        return _emptyList;
      }
    }

    public IList<IEdge> Outgoing {
      get {
        if (Enabled) {
          return _outgoing.Where(edge => ((EncounterNode)edge.End).Enabled).ToList();
        }
        return _emptyList;
      }
    }

    public bool Enabled { get; set; } = true;

    public EncounterNode(Vector3Int gridPosition) {
      GridPosition = gridPosition;
      _incoming = new List<IEdge>();
      _outgoing = new List<IEdge>();
      _emptyList = new List<IEdge>();
    }
    
    public void Connect(EncounterNode node) {
      if (_outgoing.Any(edge => edge.End == node)) {
        // Don't duplicate connections if the connection is already made.
        return;
      }
      
      // Connect bi-directional
      Edge outgoingEdge = new Edge(this, node, TraversalVelocity);
      Edge incomingEdge = new Edge(node, this, TraversalVelocity);
      _outgoing.Add(outgoingEdge);
      _incoming.Add(incomingEdge);
      node._incoming.Add(outgoingEdge);
      node._outgoing.Add(incomingEdge);
    }

    public override string ToString() => Position.ToString();
  }
}