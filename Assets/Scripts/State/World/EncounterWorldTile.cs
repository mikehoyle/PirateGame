using System;
using System.Collections.Generic;
using Common;
using State.Encounter;
using State.Unit;
using StaticConfig.Encounters;
using UnityEngine.SceneManagement;
using Zen.Hexagons;

namespace State.World {
  [Serializable]
  public class EncounterWorldTile : WorldTile {
    public bool isInitialized;
    public SparseMatrix3d<TerrainType> terrain;
    public SparseMatrix3d<CollectableInstance> collectables;
    public SparseMatrix3d<ObstacleConfig> obstacles;
    public List<UnitEncounterState> enemies;

    public EncounterWorldTile(HexOffsetCoordinates coordinates) : base(coordinates) {
      isTraversable = true;
      connectsToBoundaries = true;
    }

    public void MarkDefeated() {
      state = TileState.Cleared;
      isTraversable = false;
    }

    public override void OnVisit() {
      base.OnVisit();
      SceneManager.LoadScene(Scenes.Name.Encounter.SceneName());
    }
  }
  
  [Serializable]
  public enum TerrainType {
    Land,
  }
}