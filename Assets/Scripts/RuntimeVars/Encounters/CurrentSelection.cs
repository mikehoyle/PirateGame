﻿using Encounters;
using Optional;
using Optional.Unsafe;
using Units.Abilities;
using UnityEngine;

namespace RuntimeVars.Encounters {
  [CreateAssetMenu(menuName = "Encounters/CurrentSelection")]
  public class CurrentSelection : ScriptableObject {
    public Option<UnitAbility> selectedAbility;
    public Option<EncounterActor> selectedUnit;

    public bool TryGet(out UnitAbility ability, out EncounterActor unit) {
      if (selectedAbility.HasValue && selectedUnit.HasValue) {
        ability = selectedAbility.ValueOrFailure();
        unit = selectedUnit.ValueOrFailure();
        return true;
      }
      ability = null;
      unit = null;
      return false;
    }

    public bool TryGetUnit(out EncounterActor unit) {
      if (selectedUnit.HasValue) {
        unit = selectedUnit.ValueOrFailure();
        return true;
      }

      unit = null;
      return false;
    }
    
    public void Reset() {
      selectedAbility = Option.None<UnitAbility>();
      selectedUnit = Option.None<EncounterActor>();
    }
  }
}