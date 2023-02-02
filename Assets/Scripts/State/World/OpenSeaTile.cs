using System;

namespace State.World {
  [Serializable]
  public class OpenSeaTile : WorldTile {
    public override Type TileType => Type.OpenSea;
  }
}