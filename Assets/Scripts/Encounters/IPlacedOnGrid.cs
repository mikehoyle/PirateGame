using UnityEngine;

namespace Encounters {
  public interface IPlacedOnGrid {
    public Vector3Int Position { get; set; }
  }
}