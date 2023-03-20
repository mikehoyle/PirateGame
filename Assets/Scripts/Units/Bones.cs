using System;
using System.Collections;
using System.Collections.Generic;
using Common.Grid;
using Encounters;
using HUD.Encounter.HoverDetails;
using State.Unit;
using StaticConfig.Units;
using UnityEngine;

namespace Units {
  public class Bones : MonoBehaviour, IPlacedOnGrid, IDisplayDetailsProvider {
    [SerializeField] private ExhaustibleResources exhaustibleResources;
    [SerializeField] private float percentHpOnRevival = 0.25f;
    [SerializeField] private SerializableDictionary<ExhaustibleResource, int> collectionCost;

    private UnitEncounterState _deadUnit;

    public UnitEncounterState DeadUnit => _deadUnit;
    public Vector3Int Position { get; private set; }
    public bool BlocksAllMovement => false;
    public bool ClaimsTile => false;
    public bool BlocksLineOfSight => false;

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