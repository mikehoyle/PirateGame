using System;
using System.Collections.Generic;
using Common;
using StaticConfig;
using StaticConfig.Builds;
using UnityEngine;

namespace State {
  /// <summary>
  /// Tracks state of the ship and what is constructed on it
  /// </summary>
  [CreateAssetMenu(menuName = "State/ShipState")]
  public class ShipState : ScriptableObject {
    /// <summary>
    /// Built ship components in 3D space, starting at Z=0.
    /// Avoid modifying this directly.
    /// </summary>
    public SparseMatrix3d<ConstructableObject> components;

    public void Add(Vector3Int coords, ConstructableObject constructable) {
      components.Add(coords, constructable);
    }
  }
}