using System;
using System.Collections.Generic;
using System.Linq;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Primitives;
using UnityEngine;
using Edge = Roy_T.AStar.Graphs.Edge;

namespace Terrain {
  /// <summary>
  /// Represents a single tile in the terrain of the scene.
  /// </summary>
  public class TerrainTile : MonoBehaviour, INode {
    public static readonly Velocity TraversalVelocity = Velocity.FromMetersPerSecond(1);
    
    private IList<IEdge> _incoming;
    private IList<IEdge> _outgoing;
    private IList<IEdge> _emptyList;
    private SpriteRenderer _sprite;

    public Vector3Int GridPosition { get; private set; }

    public Position Position => new(GridPosition.x, GridPosition.y);

    public bool Enabled { get; set; } = true;

    public IList<IEdge> Incoming {
      get {
        if (Enabled) {
          return _incoming.Where(edge => ((TerrainTile)edge.Start).Enabled).ToList();
        }
        return _emptyList;
      }
    }

    public IList<IEdge> Outgoing {
      get {
        if (Enabled) {
          return _outgoing.Where(edge => ((TerrainTile)edge.End).Enabled).ToList();
        }
        return _emptyList;
      }
    }

    private void Awake() {
      _incoming = new List<IEdge>();
      _outgoing = new List<IEdge>();
      _emptyList = new List<IEdge>();
      _sprite = GetComponent<SpriteRenderer>();
    }

    private void Update() {
      Enabled = !SceneTerrain.IsMovementBlocked(GridPosition);
    }

    public void Initialize(Vector3Int position, Sprite sprite) {
      GridPosition = position;
      _sprite.sprite = sprite;
      transform.position = SceneTerrain.CellAnchorWorldStatic(position);
      gameObject.isStatic = true;
    }
    
    public void Connect(TerrainTile node) {
      if (_outgoing.Any(edge => (TerrainTile)edge.End == node)) {
        // Don't duplicate connections if the connection is already made.
        Debug.Log("Detected duplicate connection attempt");
        return;
      }
      
      // Connect bi-directional
      var outgoingEdge = new Edge(this, node, TraversalVelocity);
      var incomingEdge = new Edge(node, this, TraversalVelocity);
      _outgoing.Add(outgoingEdge);
      _incoming.Add(incomingEdge);
      node._incoming.Add(outgoingEdge);
      node._outgoing.Add(incomingEdge);
    }
    
    public void Remove() {
      _incoming.Clear();
      
      for (int index = _outgoing.Count - 1; index >= 0; --index) {
        var edge = _outgoing[index];
        _outgoing.Remove(edge);
        ((TerrainTile)edge.End)._incoming.Remove(edge);
      }
      
      Destroy(gameObject);
    }

    public override string ToString() => GridPosition.ToString();

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) {
        return false;
      }
      if (ReferenceEquals(this, obj)) {
        return true;
      }
      if (obj.GetType() != this.GetType()) {
        return false;
      }
      return Equals((TerrainTile)obj);
    }
    
    private bool Equals(TerrainTile other) {
      return base.Equals(other) && GridPosition.Equals(other.GridPosition);
    }
    
    public override int GetHashCode() {
      return HashCode.Combine(base.GetHashCode(), GridPosition);
    }
  }
}