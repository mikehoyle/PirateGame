using System.Collections.Generic;
using StaticConfig.Equipment;
using Units.Abilities;
using UnityEngine;

namespace State.Unit {
  // TODO(P1): Create reasonable separation for far-less-capable enemy units or NPCs.
  [CreateAssetMenu(menuName = "State/UnitState")]
  public class UnitState : ScriptableObject {
    public string firstName;
    public string lastName;

    public Vector3Int startingPosition;
    public UnitEncounterState encounterState;
    public SerializableDictionary<EquipmentSlot, Equipment> equipped;

    public List<UnitAbility> GetAbilities() {
      List<UnitAbility> unitAbilities = new();
      foreach (var equipment in equipped.Values) {
        unitAbilities.AddRange(equipment.abilitiesProvided);
      }
      return unitAbilities;
    }

    public string GetName() {
      return firstName + " " + lastName;
    }
  }
}