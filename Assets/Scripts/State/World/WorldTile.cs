using System;
using UnityEngine;

namespace State.World {
  /// <summary>
  /// TODO definitely rework this entire class as the concept of world tiles is expanded.
  /// </summary>
  [Serializable]
  public abstract class WorldTile {
    public enum Type {
      OpenSea,
      Tavern,
      Encounter,
    }

    public WorldCoordinates Coordinates;
    public abstract Type TileType { get; }

    public T DownRef<T>() where T : WorldTile {
      if (this is T downRef) {
        return downRef;
      }
      
      Debug.LogWarning($"Cannot cast tile of type {TileType} as {nameof(T)}");
      return null;
    } 
  }
}