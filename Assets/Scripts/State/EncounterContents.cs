using System;
using System.Collections.Generic;
using UnityEngine;

namespace State {
  /// <summary>
  /// Describes the contents a particular grid-based encounter on a tile.
  /// </summary>
  [Serializable]
  public class EncounterContents {
    public List<TerrainTile> Terrain;
    public List<UnitState> Units;

    [Serializable]
    public struct TerrainTile {
      public TerrainType Type;
      public Vector3Int GridPosition;
    }

    public enum TerrainType {
      Land,
    }
  }
}