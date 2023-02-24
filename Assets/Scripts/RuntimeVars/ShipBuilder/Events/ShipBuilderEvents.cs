using Common.Events;
using RuntimeVars.Encounters.Events;
using UnityEngine;

namespace RuntimeVars.ShipBuilder.Events {
  [CreateAssetMenu(menuName = "Events/ShipBuilder/ShipBuilderEvents")]
  public class ShipBuilderEvents : ScriptableObject {
    public BuildSelectedEvent buildSelected;
    public EmptyGameEvent enterConstructionMode;
    public EmptyGameEvent exitConstructionMode;
    public PlayerUnitEvent unitSelected;
    public ObjectClickedEvent objectClicked;
    public PlayerUnitEvent openCharacterSheet;
    public EmptyGameEvent closeCharacterSheet;
    public PlayerUnitEvent unitLevelUpStat;
    public BuildSelectedEvent inGameBuildClicked;
    public EmptyGameEvent closeCraftingMenu;
    public EquipmentEvent equipmentCraftedEvent;
    public EquipmentEvent attemptEquipItem;
    public EquipmentEvent itemEquipped;
  }
}