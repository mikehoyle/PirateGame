using System;
using System.Collections.Generic;
using UnityEngine;
using Zen.Hexagons;

namespace Common.Grid {
  /// <summary>
  /// Represents an edge in a hex grid, Defined here as the unique connecting line
  /// between two adjacent cells.
  /// </summary>
  [Serializable]
  public struct HexEdge {
    public HexOffsetCoordinates borderCellOne;
    public HexOffsetCoordinates borderCellTwo;

    public HexEdge(HexOffsetCoordinates borderCellOne, HexOffsetCoordinates borderCellTwo) {
      this.borderCellOne = borderCellOne;
      this.borderCellTwo = borderCellTwo;
    }

    // For now doesn't calculate path, just direct crossing
    public bool CrossesBorder(HexOffsetCoordinates origin, HexOffsetCoordinates destination) {
      return (origin == borderCellOne && destination == borderCellTwo)
          || (origin == borderCellTwo && destination == borderCellOne);
    }

    public List<Vector3> WorldVertices(Func<Vector3Int, Vector3> cellCenterWorld) {
      var verticesOne = HexGridUtils.HexTileVertices(cellCenterWorld(borderCellOne.AsVector3Int()));
      var verticesTwo = HexGridUtils.HexTileVertices(cellCenterWorld(borderCellTwo.AsVector3Int()));

      var commonVertices = new List<Vector3>();
      foreach (var vertex in verticesOne) {
        foreach (var vertex2 in verticesTwo) {
          if (vertex.AboutEquals(vertex2)) {
            commonVertices.Add(vertex);
            break;
          }
        }
      }
      
      if (commonVertices.Count != 2) {
        Debug.LogWarning($"Expecting two common vertices for any adjacent tiles. {borderCellOne}, {borderCellTwo}");
        Debug.LogWarning($"Instead found {commonVertices.Count}, {string.Join(", ", commonVertices)}");
        Debug.LogWarning($"First vertices set was {string.Join(", ", verticesOne)}");
        Debug.LogWarning($"Second vertices set was {string.Join(", ", verticesTwo)}");
      }
      
      return commonVertices;
    }
    
    public override string ToString() {
      return $"{nameof(borderCellOne)}: {borderCellOne}, {nameof(borderCellTwo)}: {borderCellTwo}";
    }
  }
}