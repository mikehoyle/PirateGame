using System;

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
  }
}