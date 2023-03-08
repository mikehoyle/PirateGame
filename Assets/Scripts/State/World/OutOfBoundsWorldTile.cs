using System;
using Zen.Hexagons;

namespace State.World {
  [Serializable]
  public class OutOfBoundsWorldTile : WorldTile {
    public OutOfBoundsWorldTile(HexOffsetCoordinates coordinates) : base(coordinates) {
      isTraversable = false;
    }
  }
}