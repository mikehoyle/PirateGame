using UnityEngine;

namespace Encounters {
  public interface IPlacedOnGrid {
    public Vector3Int Position { get; }
    public bool BlocksAllMovement { get; }
    public bool ClaimsTile { get; }
    public bool BlocksLineOfSight { get; }
  }
}