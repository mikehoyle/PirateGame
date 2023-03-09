using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zen.Hexagons;

namespace Common.Grid {
  /// <summary>
  /// Calculates the path between two hex cells. To accomplish this, we use the following strategy:
  ///  - Given hex cells A and B.
  ///  - Find a shortest-valid line of hexes between A and B.
  ///  - For each hex in that line, find the "short side" of open tiles between the tile before it and the tile after it
  ///  - Create an edge for each open cell on that short side.
  ///  - To cover cases where the edge crosses through the two neighboring hexes, ensure the next short-side neighbor set
  ///       always includes the first short-side neighbor set.  
  /// </summary>
  public class HexPathGenerator {
    private static readonly List<Direction> Directions = new() {
        Direction.NorthWest,
        Direction.NorthEast,
        Direction.East,
        Direction.SouthEast,
        Direction.SouthWest,
        Direction.West,
    };

    public HexPath GeneratePath(HexOffsetCoordinates fromCell, HexOffsetCoordinates toCell) {
      var result = new List<HexEdge>();
      var path = HexGridUtils.HexLibrary.GetLine(fromCell, toCell);

      // Ignore first and last, iterate through the cells between.
      for (var i = 1; i < path.Length - 1; i++) {
        foreach (var neighbor in GetShortSideNeighbors(result, path[i], path[i - 1], path[i + 1], i != 1)) {
          result.Add(new HexEdge(neighbor, path[i]));
        }
      }

      return new HexPath {
          endpointOne = fromCell,
          endpointTwo = toCell,
          edges = result,
      };
    }

    private List<HexOffsetCoordinates> GetShortSideNeighbors(
        List<HexEdge> currentEdges,
        HexOffsetCoordinates cell,
        HexOffsetCoordinates previousNeighbor,
        HexOffsetCoordinates nextNeighbor,
        bool handleContinuity) {
      var neighborOneDirection = GetDirectionIndex(cell, previousNeighbor);
      var neighborTwoDirection = GetDirectionIndex(cell, nextNeighbor);
      if (neighborOneDirection == -1 || neighborTwoDirection == -1) {
        return new List<HexOffsetCoordinates>();
      }

      var setOne = GetNeighborDirectionsBetween(neighborOneDirection, neighborTwoDirection);
      var setTwo = GetNeighborDirectionsBetween(neighborTwoDirection, neighborOneDirection);

      var workingSet = setOne.Count switch {
          // When both sides are of equal length, pick whichever one has a higher-indexed direction in it.
          // This should mean there's a tendency for all paths to err in the same direction, and therefore
          // overlap more, instead of creating weird 1-hex pockets.
          var count when count == setTwo.Count => setOne.Max() > setTwo.Max() ? setOne : setTwo,
          var count when count > setTwo.Count => setTwo,
          _ => setOne,
      };
      
      var result = workingSet
          .Select(directionIndex => HexGridUtils.HexLibrary.GetNeighbor(cell, Directions[directionIndex]))
          .ToList();
      
      // Handle weirdness where we may be switching "sides" and need a crossover edge. There's zero chance
      // I even remember what this means in the future so let's pray it never breaks.
      if (handleContinuity) {
        var isContinuousPath = false;
        foreach (var edge in currentEdges) {
          if (result.Contains(edge.borderCellOne) || result.Contains(edge.borderCellTwo)) {
            isContinuousPath = true;
          }
        }

        if (!isContinuousPath) {
          result.Add(previousNeighbor);
        }
      }
      
      return result;
    }

    private int GetDirectionIndex(HexOffsetCoordinates reference, HexOffsetCoordinates adjacentCell) {
      for (int i = 0; i < Directions.Count; i++) {
        if (HexGridUtils.HexLibrary.GetNeighbor(reference, Directions[i]) == adjacentCell) {
          return i;
        }
      }
      Debug.LogError($"Unable to find direction, cell ({reference}) must not be adjacent to {adjacentCell}");
      return -1;
    }

    private List<int> GetNeighborDirectionsBetween(int indexOne, int indexTwo) {
      var result = new List<int>();

      var currentIndex = indexOne + 1;
      while (currentIndex % Directions.Count != indexTwo) {
        result.Add(currentIndex % Directions.Count);
        currentIndex++;
      }

      return result;
    }
  }
}