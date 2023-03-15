using State.World;

namespace Units {
  /// <summary>
  /// A place to aggregate all the highly tweaked logic around XP.
  /// </summary>
  public static class ExperienceCalculations {
    public static int GetLevelRequirement(int level) {
      return (100 * (level * level)) - (100 * level);
    }

    public static int GetXpForVictoryInEncounter(EncounterWorldTile encounterTile) {
      return (int)(encounterTile.difficulty * 20);
    }
  }
}