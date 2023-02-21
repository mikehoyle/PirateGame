using System;
using UnityEngine;

namespace StaticConfig.Encounters {
  [Serializable]
  public class ObstacleConfig {
    public int maxHp;
    public int currentHp;
    // Obstacle sprites are assumed to have a pivot at the tile base.
    public Sprite sprite;
    public Vector2Int size = Vector2Int.one;
  }
}