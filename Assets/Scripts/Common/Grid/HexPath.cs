using System;
using System.Collections.Generic;
using UnityEngine;
using Zen.Hexagons;

namespace Common.Grid {
  [Serializable]
  public class HexPath {
    public HexOffsetCoordinates endpointOne;
    public HexOffsetCoordinates endpointTwo;
    public List<HexEdge> edges;

    public List<Vector3> GetWorldPath(Func<Vector3Int, Vector3> cellCenterWorld) {
      var endpointOneWorld = cellCenterWorld(endpointOne.AsVector3Int());
      var endpointTwoWorld = cellCenterWorld(endpointTwo.AsVector3Int());
      var result = new List<Vector3>();
      result.Add(endpointOneWorld);
      result.Add(endpointTwoWorld);

      foreach (var edge in edges) {
        // There are duplicates at every internal edge... but it doesn't really matter for now,
        // just let them be.
        result.AddRange(edge.WorldVertices(cellCenterWorld));
      }
      
      // Sort by proximity to the first endpoint for what should be an in-order line
      result.Sort((first, second) => {
        var diff = Vector3.Distance(second, endpointOneWorld) - Vector3.Distance(first, endpointOneWorld);
        if (Math.Abs(diff) < 0.001) {
          return 0;
        }
        return diff > 0 ? 1 : -1;
      });
      return result;
    }

    public bool HasEndpoint(HexOffsetCoordinates coord) {
      return endpointOne == coord || endpointTwo == coord;
    }
    
    #region Equality Overrides
    protected bool Equals(HexPath other) {
      return endpointOne.Equals(other.endpointOne) && endpointTwo.Equals(other.endpointTwo);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) {
        return false;
      }
      if (ReferenceEquals(this, obj)) {
        return true;
      }
      if (obj.GetType() != this.GetType()) {
        return false;
      }
      return Equals((HexPath)obj);
    }
    public override int GetHashCode() {
      return HashCode.Combine(endpointOne, endpointTwo);
    }    

    #endregion
  }
}