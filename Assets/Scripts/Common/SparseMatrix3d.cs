using System;
using UnityEngine;

namespace Common {
  /// <summary>
  /// OPTIMIZE: this shouldn't be harsh on memory usage, but may have subpar lookup performance due to hashing overhead.
  ///     It's probably within reasonable bounds, though.
  /// </summary>
  [Serializable]
  public class SparseMatrix3d<T> : SerializableDictionary<Vector3Int, T> {
    // Ignores the Z dimension
    public RectInt GetBoundingRect() {
      if (Count == 0) {
        return new RectInt(0, 0, 0, 0);
      }

      var minX = int.MaxValue;
      var minY = int.MaxValue;
      var maxX = int.MinValue;
      var maxY = int.MinValue;

      foreach (var coord in Keys) {
        minX = Math.Min(coord.x, minX);
        minY = Math.Min(coord.y, minY);
        maxX = Math.Max(coord.x, maxX);
        maxY = Math.Max(coord.y, maxY);
      }

      return new RectInt(minX, minY, maxX - minX, maxY - minY);
    }
  }
}