using System;
using System.Collections.Generic;
using Common;

namespace State.World {
  [Serializable]
  public class EncounterTile : WorldTile {
    public bool IsInitialized;
    
    public SparseMatrix3d<TerrainType> Terrain = new();
    public List<UnitState> Units = new();
    
    public override Type TileType => Type.Encounter;
  }
  
  [Serializable]
  public enum TerrainType {
    Land,
  }
}