using System;
using System.Collections.Generic;
using Common;
using State.Encounter;
using State.Unit;
using StaticConfig.Encounters;
using UnityEngine;

namespace State.World {
  [CreateAssetMenu(menuName = "State/EncounterTile")]
  public class EncounterTile : WorldTile {
    public bool isInitialized;
    public int difficultyRating;
    public SparseMatrix3d<TerrainType> terrain;
    public SparseMatrix3d<CollectableInstance> collectables;
    public SparseMatrix3d<ObstacleConfig> obstacles;
    public List<UnitEncounterState> enemies;
    
    public override Type TileType => Type.Encounter;
  }
  
  [Serializable]
  public enum TerrainType {
    Land,
  }
}