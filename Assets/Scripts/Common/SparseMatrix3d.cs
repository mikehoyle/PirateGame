using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Common {
  /// <summary>
  /// OPTIMIZE: this shouldn't be harsh on memory usage, but may have subpar lookup performance due to hashing overhead.
  ///     It's probably within reasonable bounds, though.
  /// </summary>
  [Serializable]
  public class SparseMatrix3d<T> : IDictionary<Vector3Int, T> {
    /// <summary>
    /// This is a messy conversion, but necessary because Vector3Int isn't as fully featured as we'd like.
    /// </summary>
    [Serializable]
    private struct Coordinate : IEquatable<Coordinate> {
      public int x;
      public int y;
      public int z;

      public Coordinate(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
      }

      public Vector3Int AsVector3Int() {
        return new Vector3Int(x, y, z);
      }

      public static Coordinate FromVector3Int(Vector3Int vec) {
        return new Coordinate(vec.x, vec.y, vec.z);
      }
      
      public bool Equals(Coordinate other) {
        return x == other.x && y == other.y && z == other.z;
      }
      public override bool Equals(object obj) {
        return obj is Coordinate other && Equals(other);
      }
      public override int GetHashCode() {
        return HashCode.Combine(x, y, z);
      }
    }

    private Dictionary<Coordinate, T> _data = new();
    
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

    // Tons of Collection interface boilerplate below
    public IEnumerator<KeyValuePair<Vector3Int, T>> GetEnumerator() {
      return _data.Select(entry => KeyValuePair.Create(entry.Key.AsVector3Int(), entry.Value))
          .GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
    public void Add(KeyValuePair<Vector3Int, T> item) {
      _data.Add(Coordinate.FromVector3Int(item.Key), item.Value);
    }
    public void Clear() {
      _data.Clear();
    }
    public bool Contains(KeyValuePair<Vector3Int, T> item) {
      return _data.Contains(KeyValuePair.Create(Coordinate.FromVector3Int(item.Key), item.Value));
    }
    public void CopyTo(KeyValuePair<Vector3Int, T>[] array, int arrayIndex) {
      // unimplemented
    }
    public bool Remove(KeyValuePair<Vector3Int, T> item) {
      return _data.Remove(Coordinate.FromVector3Int(item.Key));
    }

    public int Count => _data.Count;
    public bool IsReadOnly => false;
    public void Add(Vector3Int key, T value) {
      _data.Add(Coordinate.FromVector3Int(key), value);
    }
    public bool ContainsKey(Vector3Int key) {
      return _data.ContainsKey(Coordinate.FromVector3Int(key));
    }
    public bool Remove(Vector3Int key) {
      return _data.Remove(Coordinate.FromVector3Int(key));
    }
    public bool TryGetValue(Vector3Int key, out T value) {
      return _data.TryGetValue(Coordinate.FromVector3Int(key), out value);
    }
    public T this[Vector3Int key] {
      get => _data[Coordinate.FromVector3Int(key)];
      set => _data[Coordinate.FromVector3Int(key)] = value;
    }
    public ICollection<Vector3Int> Keys => _data.Keys.Select(key => key.AsVector3Int()).AsReadOnlyCollection();
    public ICollection<T> Values => _data.Values;
  }
}