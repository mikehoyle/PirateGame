using System;

namespace State {
  /// <summary>
  /// Holds World-level game state to be saved. 
  /// </summary>
  [Serializable]
  public class WorldState {
    public uint CurrentDay = 1;
  }
}