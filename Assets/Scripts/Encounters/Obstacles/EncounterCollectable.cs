using State;
using State.Encounter;
using State.World;
using Terrain;
using UnityEngine;

namespace Encounters.Obstacles {
  public class EncounterCollectable : MonoBehaviour, IPlacedOnGrid {
    public CollectableInstance Metadata { get; private set; }
    public Vector3Int Position { get; private set; }

    public void Initialize(CollectableInstance collectable, Vector3Int position) {
      Metadata = collectable;
      Position = position;
      transform.position = SceneTerrain.CellBaseWorldStatic(position);
    }
    
    
    // For now, just send straight to player inventory and remove self
    public void Collect() {
      Metadata.AddToPlayerInventory();
      var encounter = GameState.State.world.GetActiveTile().DownCast<EncounterTile>();
      encounter.collectables.Remove(Position);
      Destroy(gameObject);
    }
  }
}