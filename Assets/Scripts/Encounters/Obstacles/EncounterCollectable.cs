using System.Collections.Generic;
using Common.Animation;
using Common.Grid;
using HUD.Encounter.HoverDetails;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using State.Encounter;
using Units;
using UnityEngine;

namespace Encounters.Obstacles {
  public class EncounterCollectable : MonoBehaviour, IPlacedOnGrid, IDisplayDetailsProvider, IDirectionalAnimatable {
    [SerializeField] private int collectionRange = 1;
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private CurrentSelection currentSelection;

    public CollectableInstance Metadata { get; private set; }
    public Vector3Int Position { get; private set; }
    public bool BlocksAllMovement => true;
    public bool ClaimsTile => true;
    public bool BlocksLineOfSight => true;
    public FacingDirection FacingDirection => FacingDirection.SouthWest;
    public string AnimationState => "idle";
    public event IDirectionalAnimatable.RequestOneOffAnimation OneOffAnimation;
    
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
      transform.position = GridUtils.CellCenterWorldStatic(position);
    }

    private void TryCollect() {
      if (!currentSelection.TryGetUnit<PlayerUnitController>(out var playerActor)) {
        return;
      }
      
      var distance = GridUtils.DistanceBetween(Position, playerActor.Position);
      if (distance > collectionRange) {
        Debug.Log("Actor too far to collect");
        return;
      }
      
      // TODO(P1): Any animation/sound/anything
      playerActor.FaceTowards(Position);
      Collect(playerActor);
    }
    
    private void Collect(PlayerUnitController collector) {
      collector.AddCollectable(Metadata);
      //var encounter = GameState.State.world.GetActiveTile().DownCast<EncounterTile>();
      //encounter.collectables.Remove(Position);
      Destroy(gameObject);
    }
    
    public DisplayDetails GetDisplayDetails() {
      var additionalDetails = new List<string>();
      foreach (var resource in Metadata.contents) {
        additionalDetails.Add($"{resource.Key.displayName}: {resource.Value}");
      }
      return new DisplayDetails {
          Name = "Resource Crate",
          AdditionalDetails = additionalDetails,
      };
    }
  }
}