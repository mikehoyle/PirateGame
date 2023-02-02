using System;
using System.Collections.Generic;
using Common;

namespace State.World {
  [Serializable]
  public class EncounterTile : WorldTile {
    public SparseMatrix3d<TerrainType> Terrain;
    public List<UnitState> Units;
    
    public override Type TileType => Type.Encounter;
  }
  
  [Serializable]
  public enum TerrainType {
    Land,
  }
}