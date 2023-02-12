using System;
using System.Collections.Generic;
using System.Linq;
using Encounters;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Primitives;
using UnityEngine;

namespace Terrain {
  /// <summary>
  /// Represents a single tile in the terrain of the scene.
  /// </summary>
  public class TerrainTile : MonoBehaviour, INode {
    public static readonly Velocity TraversalVelocity = Velocity.FromMetersPerSecond(1);
    private static readonly Vector3Int CellOffset = new Vector3Int(1, 1, 0);
    
    private IList<IEdge> _incoming;
    private IList<IEdge> _outgoing;
    private IList<IEdge> _emptyList;
    private SpriteRenderer _sprite;
    private Vector3 _worldCenter;
    private LayerMask _blockingLayer;

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
      _blockingLayer = LayerMask.GetMask("BlockMovement");
    }

    private void Update() {
      var shouldBeEnabled = true;
      foreach (var collision in Physics2D.OverlapPointAll(_worldCenter, _blockingLayer)) {
        // A simple check isn't sufficient, because isometric elevation can overlay elevated tiles on top
        // of lower tiles behind them, so check the Z position of collisions, if possible
        if (collision.TryGetComponent<IPlacedOnGrid>(out var placedOnGrid)) {
          if (placedOnGrid.Position.z == GridPosition.z) {
            shouldBeEnabled = false;
          }
        } else {
          // This should hopefully not be the case, but worst case assume an intersection is blocking
          shouldBeEnabled = false;
        }
      }

      Enabled = shouldBeEnabled;
    }

    public void Initialize(Vector3Int position, Sprite sprite, Grid grid) {
      GridPosition = position;
      _sprite.sprite = sprite;
      
      // We have to adjust the position, because (bafflingly) Unity wants to sort tiles based on their
      // center, which will result in all sorts of clipping issues. This adjustement places them with the assumption
      // that their pivot is at top-center (where we place it), which resolves all sorts of woes.
      var adjustedPosition = position + CellOffset;
      _worldCenter = grid.GetCellCenterWorld(adjustedPosition);
      transform.position = grid.CellToWorld(adjustedPosition);
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