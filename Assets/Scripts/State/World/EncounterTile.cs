using System;
using System.Collections.Generic;
using Common;
using State.Unit;
using UnityEngine;

namespace State.World {
  [CreateAssetMenu(menuName = "State/EncounterTile")]
  public class EncounterTile : WorldTile {
    public bool isInitialized;
    public SparseMatrix3d<TerrainType> terrain;
    public List<EnemyUnitState> enemies;
    
    public override Type TileType => Type.Encounter;
  }
  
  [Serializable]
  public enum TerrainType {
    Land,
  }
}