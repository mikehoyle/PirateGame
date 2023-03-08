using System;

namespace State.World {
  [Serializable]
  public enum TileState {
    Obscured,
    Visible,
    VisitedButStillAvailable,
    Cleared,
  }
}