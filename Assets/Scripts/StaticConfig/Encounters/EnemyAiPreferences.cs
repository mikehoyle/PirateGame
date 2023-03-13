using System;

namespace StaticConfig.Encounters {
  [Serializable]
  public class EnemyAiPreferences {
    public float playerUnitAdjacency = 5f;
    public float distanceFromPlayerByTile = -0.1f;
    public float stayStationary = 0.05f;
    public float canPerformAbility = 10f;
    public float inRadiusOfAlly = 0f;
    public int allyRadius = 0;
  }
}