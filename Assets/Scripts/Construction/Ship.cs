using Common.Grid;
using UnityEngine;

namespace Construction {
  public class Ship : MonoBehaviour {
    private static readonly Vector3Int SpriteOffset = new(-6, 0);


    public void Initialize(Vector3Int shipOffset) {
      transform.position = GridUtils.CellCenterWorld(shipOffset + SpriteOffset);
    }
  }
}