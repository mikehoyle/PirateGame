using System.Collections.Generic;
using Common.Grid;
using Encounters;
using HUD.Encounter.HoverDetails;
using State.Unit;
using UnityEngine;

namespace Units {
  public class Bones : MonoBehaviour, IPlacedOnGrid, IDisplayDetailsProvider {
    private UnitEncounterState _deadUnit;

    public UnitEncounterState DeadUnit => _deadUnit;
    public Vector3Int Position { get; private set; }

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
  }
}