using System;

namespace State.World {
  [Serializable]
  public class TavernTile : WorldTile {
    public override Type TileType => Type.Tavern;
    
    // TODO any specifics
  }
}