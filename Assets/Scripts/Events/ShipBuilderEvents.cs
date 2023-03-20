using Common.Events;
using Encounters;
using StaticConfig.Builds;
using StaticConfig.Equipment;
using UnityEngine;

namespace Events {
  public class ShipBuilderEvents {
    public readonly GameEvent<ConstructableObject> BuildSelected = new();
    public readonly GameEvent EnterConstructionMode = new();
    public readonly GameEvent ExitConstructionMode = new();
    public readonly GameEvent<EncounterActor> UnitSelected = new();
    public readonly GameEvent<GameObject> ObjectClicked = new();
    public readonly GameEvent<EncounterActor> OpenCharacterSheet = new();
    public readonly GameEvent CloseCharacterSheet = new();
    public readonly GameEvent<EncounterActor> UnitLevelUpStat = new();
    public readonly GameEvent<ConstructableObject> InGameBuildClicked = new();
    public readonly GameEvent CloseCraftingMenu = new();
    public readonly GameEvent<EquipmentItemInstance> EquipmentCraftedEvent = new();
    public readonly GameEvent<EquipmentItemInstance> AttemptEquipItem = new();
    public readonly GameEvent<EquipmentItemInstance> ItemEquipped = new();
  }
}