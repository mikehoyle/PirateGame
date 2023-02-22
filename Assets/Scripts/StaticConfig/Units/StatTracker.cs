using System;
using UnityEngine;

namespace StaticConfig.Units {
  [Serializable]
  public class StatTracker {
    public Stat stat;
    public int current;
    
    public static StatTracker NewTracker(Stat stat, int current) {
      return new StatTracker() {
          stat = stat,
          current = current,
      };
    }

    public void LevelUp() {
      if (!CanBeLeveledUp()) {
        Debug.LogWarning($"Cannot level up beyond max possible value for stat {stat}");
        return;
      }
      
      current++;
    }

    public bool CanBeLeveledUp() {
      return current < stat.maxValue;
    }
  }
}