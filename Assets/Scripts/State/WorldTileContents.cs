using System;
using JetBrains.Annotations;

namespace State {
  /// <summary>
  /// TODO definitely rework this entire class as the concept of world tiles is expanded.
  /// </summary>
  [Serializable]
  public class WorldTileContents {
    public enum Type {
      OpenSea,
      Tavern,
      Encounter,
    }

    public Type TileType;
    [CanBeNull] public EncounterContents EncounterContents;
  }
}