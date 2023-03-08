using System;
using Zen.Hexagons;

namespace State.World {
  [Serializable]
  public class OpenSeaWorldTile : WorldTile {
    public OpenSeaWorldTile(HexOffsetCoordinates coordinates) : base(coordinates) {
      isTraversable = true;
    }
  }
}