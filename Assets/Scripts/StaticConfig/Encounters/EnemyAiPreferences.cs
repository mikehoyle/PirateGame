using System;

namespace StaticConfig.Encounters {
  [Serializable]
  public class EnemyAiPreferences {
    public float atPreferredRangeFromPlayer = 5f;
    public int preferredRangeFromPlayer = 1;
    public float dropOffFromPreferredRangeByTile = 0.1f;
    public float stayStationary = 0.05f;
    public float canPerformAbility = 10f;
    public float inRadiusOfAlly = 0f;
    public int allyRadius = 0;
  }
}