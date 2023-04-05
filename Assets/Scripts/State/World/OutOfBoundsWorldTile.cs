using System;
using Zen.Hexagons;

namespace State.World {
  [Serializable]
  public class OutOfBoundsWorldTile : WorldTile {
    public OutOfBoundsWorldTile(
        HexOffsetCoordinates coordinates, bool isConnectable = false) : base(coordinates) {
      isTraversable = false;
      connectsToBoundaries = isConnectable;
    }
  }
}