using NUnit.Framework;
using Common.Grid;
using UnityEngine;
using Zen.Hexagons;

namespace Tests {
  // This isn't really a test, just an easy way to debug.
  public class HexPathGeneratorTest {
    [Test]
    public void TestSimpleBorder() {
      var generator = new HexPathGenerator();
      var fromCell = new HexOffsetCoordinates(0, 0);
      var toCell = new HexOffsetCoordinates(3, 0);
      var result = generator.GeneratePath(fromCell, toCell);
      Debug.Log($"Result: {string.Join(",\n", result.edges)}");
    }
  }
}
