using System;
using UnityEngine;

namespace StaticConfig.Encounters {
  [Serializable]
  public class ObstacleConfig {
    public int maxHp;
    public int currentHp;
    public Sprite sprite;
    public Vector2Int size;
  }
}