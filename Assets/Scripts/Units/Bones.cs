using System;
using System.Collections;
using System.Collections.Generic;
using Common.Grid;
using Encounters;
using HUD.Encounter.HoverDetails;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using State.Encounter;
using State.Unit;
using StaticConfig.RawResources;
using StaticConfig.Units;
using UnityEngine;

namespace Units {
  public class Bones : MonoBehaviour, IPlacedOnGrid, IDisplayDetailsProvider {
    [SerializeField] private ExhaustibleResources exhaustibleResources;
    [SerializeField] private float percentHpOnRevival = 0.25f;
    [SerializeField] private int collectionRange = 1;
    [SerializeField] private CollectedResources collectedResources;
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private RawResource bonesResource;
    [SerializeField] private SerializableDictionary<ExhaustibleResource, int> collectionCost;

    private UnitEncounterState _deadUnit;

    public UnitEncounterState DeadUnit => _deadUnit;
    public Vector3Int Position { get; private set; }
    public bool BlocksAllMovement => false;
    public bool ClaimsTile => true;
    
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
    
    private void TryCollect() {
      if (!currentSelection.TryGetUnit<PlayerUnitController>(out var playerActor) || !CanAfford(playerActor)) {
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

    private bool CanAfford(PlayerUnitController actor) {
      foreach (var cost in collectionCost) {
        if (actor.EncounterState.GetResourceAmount(cost.Key) < cost.Value) {
          return false;
        }
      }
      return true;
    }

    private void PayCost(PlayerUnitController actor) {
      foreach (var cost in collectionCost) {
        actor.ExpendResource(cost.Key, cost.Value);
      }
    } 

    private void Collect(PlayerUnitController actor) {
      PayCost(actor);
      collectedResources.Add(new CollectableInstance {
         contents = new() {
             [bonesResource] =
                 (int)(10 * ((EnemyUnitMetadata)(_deadUnit.metadata)).spawnConfig.individualDifficultyRating),
         }
      });
      Destroy(gameObject);
    }

    public void Initialize(UnitEncounterState unit, Vector3Int position) {
      _deadUnit = unit;
      Position = position;
      transform.position = GridUtils.CellCenterWorldStatic(position);
    }

    public DisplayDetails GetDisplayDetails() {
      return new DisplayDetails {
          Name = "Bones",
          AdditionalDetails = new List<string> {
              $"({_deadUnit.metadata.GetName()})",
          },
      };
    }

    public IEnumerator ReviveUnit() {
      _deadUnit.NewEncounter();
      _deadUnit.position = Position;
      if (_deadUnit.TryGetResourceTracker(exhaustibleResources.hp, out var hpTracker)) {
        hpTracker.current = Math.Max(1, (int)(hpTracker.max * percentHpOnRevival));
      } else {
        Debug.LogError("Reviving unit did not have HP tracker. This should never happen");
      }
      
      // TODO(P1) animate, sfx and all that.
      Instantiate(_deadUnit.metadata.prefab).GetComponent<EncounterActor>().Init(_deadUnit);
      Destroy(gameObject);
      yield break;
    }
  }
}