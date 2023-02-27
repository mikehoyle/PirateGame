﻿using UnityEngine;

namespace State.World {
  /// <summary>
  /// TODO definitely rework this entire class as the concept of world tiles is expanded.
  /// </summary>
  public abstract class WorldTile : ScriptableObject {
    public enum Type {
      OpenSea,
      Encounter,
      Heart
    }


    public int difficulty { get; set; } = 0;
    public bool IsCovered { get; set; } = true;
    public WorldCoordinates coordinates;
    public abstract Type TileType { get; }

    public T DownCast<T>() where T : WorldTile {
      if (this is T downRef) {
        return downRef;
      }
      
      Debug.LogWarning($"Cannot cast tile of type {TileType} as {nameof(T)}");
      return null;
    }
  }
}