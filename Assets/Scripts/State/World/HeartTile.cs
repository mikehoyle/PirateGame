using System;

namespace State.World {
  [Serializable]
  public class HeartTile : WorldTile {
    public override Type TileType => Type.Heart;
  }
}