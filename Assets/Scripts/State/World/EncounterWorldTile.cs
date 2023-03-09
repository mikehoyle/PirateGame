using System;
using System.Collections.Generic;
using Common;
using Common.Loading;
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
      connectsToBoundaries = false;

      GameState.State.world.outpostBorders.RemoveWhere(border => border.HasEndpoint(coordinates));
    }

    public override void OnVisit() {
      base.OnVisit();
      if (state != TileState.Cleared) {
        SceneManager.LoadScene(Scenes.Name.Encounter.SceneName());
      }
    }
  }
  
  [Serializable]
  public enum TerrainType {
    Land,
  }
}