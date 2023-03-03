using UnityEngine;

namespace State.World {
  [CreateAssetMenu(menuName = "State/DefeatedEncounterTile")]
  public class DefeatedEncounterTile : WorldTile {
    public override Type TileType => Type.DefeatedEncounter;
  }
}