using System;
using System.Collections.Generic;
using UnityEngine;

namespace State {
  /// <summary>
  /// Tracks state of the ship and what is constructed on it
  /// </summary>
  [Serializable]
  public class ShipState {
    // Where base-level raft tiles are.
    public HashSet<Vector3Int> Foundations = new();
    
    // TODO: fields for contents of ship
  }
}