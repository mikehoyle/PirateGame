using System;
using Common;
using StaticConfig;
using UnityEngine;

namespace State {
  /// <summary>
  /// Tracks state of the ship and what is constructed on it
  /// </summary>
  [Serializable]
  public class ShipState {
    /// <summary>
    /// Built ship components in 3D space, starting at Z=0.
    /// The value is a ConstructableScriptedObject.id.
    /// Avoid modifying this directly.
    /// </summary>
    public SparseMatrix3d<string> Components = new();

    public void Add(Vector3Int coords, ConstructableScriptableObject constructable) {
      Components.Add(coords, constructable.id);
    }
  }
}