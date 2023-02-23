using Common;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using State.Encounter;
using Terrain;
using Units;
using UnityEngine;

namespace Encounters.Obstacles {
  public class EncounterCollectable : MonoBehaviour, IPlacedOnGrid {
    [SerializeField] private int collectionRange = 1;
    [SerializeField] CollectedResources collectedResources;
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private CurrentSelection currentSelection;

    public CollectableInstance Metadata { get; private set; }
    public Vector3Int Position { get; private set; }
    
    private void OnEnable() {
      encounterEvents.objectClicked.RegisterListener(OnObjectClicked);
    }

    private void OnDisable() {
      encounterEvents.objectClicked.UnregisterListener(OnObjectClicked);
    }

    private void OnObjectClicked(GameObject clickedObject) {
      if (clickedObject == gameObject) {
        TryCollect();
      }
    }

    public void Initialize(CollectableInstance collectable, Vector3Int position) {
      Metadata = collectable;
      Position = position;
      transform.position = SceneTerrain.CellBaseWorldStatic(position);
    }

    private void TryCollect() {
      if (!currentSelection.selectedUnit.TryGet(out var actor)
          || actor is not UnitController playerActor) {
        return;
      }
      
      var distance = GridUtils.DistanceBetween(Position, playerActor.Position);
      if (distance > collectionRange) {
        Debug.Log("Actor too far to collect");
        return;
      }
      
      // TODO(P1): Any animation/sound/anything
      Collect();
    }
    
    private void Collect() {
      collectedResources.Add(Metadata);
      //var encounter = GameState.State.world.GetActiveTile().DownCast<EncounterTile>();
      //encounter.collectables.Remove(Position);
      Destroy(gameObject);
    }
  }
}