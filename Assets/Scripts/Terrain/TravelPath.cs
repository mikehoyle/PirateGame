using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Terrain {
  public class TravelPath {
    public LinkedList<Vector3Int> Path { get; }

    public TravelPath([CanBeNull] LinkedList<Vector3Int> path) {
      Path = path;
    }

    public bool IsViable() {
      return Path != null && Path.Count > 1;
    }

    public int Length() {
      // Minus one because path includes origin
      return Path == null ? 0 : Math.Max(0, Path.Count - 1);
    }

    public bool IsViableAndWithinRange(int movementRange) {
      return IsViable() && Length() <= movementRange;
    }
  }
}