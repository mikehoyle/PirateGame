using System;

namespace StaticConfig.Encounters {
  /// <summary>
  /// Defines the parameters around which an enemy can be included in an encounter.
  ///
  /// TODO(P2): Much configuration will be needed here to create fresh and unique encounters
  /// Ideas:
  /// - Biome-preferred spawns
  /// - Common/required ally types
  /// </summary>
  [Serializable]
  public class EnemySpawnConfig {
    private const float MaxPrevalenceWeight = 50f;
    private const float MinPrevalenceWeight = 1f;
    
    public float minDifficultyRating;
    public float maxDifficultyRating;
    public float mostCommonDifficultyRating;
    // Expected to likely be somewhere between 0-2, i.e. 0-200%
    public float prevalenceMultiplier;
    public float individualDifficultyRating;
    public int maxPerEncounter;

    /// <summary>
    /// Gets the likelihood of unit spawn, as defined by a float weight, which will
    /// be compared in a pool with other candidates.
    ///
    /// Some general impressions of the meanings of weights:
    /// 0: Impossible
    /// 1: Very unlikely
    /// 10: Reasonably possible
    /// 50: Typical
    /// 100: Very likely
    /// </summary>
    public float GetSpawnWeight(EnemySpawnVariables vars) {
      if (vars.DifficultyRating < minDifficultyRating
          || vars.DifficultyRating > maxDifficultyRating) {
        return 0f;
      }

      var weightScale = (float)vars.DifficultyRating switch {
          var x when x <= mostCommonDifficultyRating =>
              (x - minDifficultyRating) / (mostCommonDifficultyRating - minDifficultyRating),
          var x => (x - mostCommonDifficultyRating) / (maxDifficultyRating - mostCommonDifficultyRating),
      };

      return PositionInScale(MinPrevalenceWeight, MaxPrevalenceWeight, weightScale) * prevalenceMultiplier;
    }

    private float PositionInScale(float scaleMin, float scaleMax, float percentage) {
      return ((scaleMax - scaleMin) * percentage) + scaleMin; 
    }
  }
}