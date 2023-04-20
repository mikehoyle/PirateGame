using System.Collections.Generic;
using System.Linq;
using Common;
using Encounters;
using Events;
using Optional;
using RuntimeVars.Encounters;
using State;
using StaticConfig.Equipment;
using StaticConfig.RawResources;
using UnityEngine;
using UnityEngine.UIElements;

namespace Units.UI {
  public class UnitUpgradeDisplay : MonoBehaviour {
    [SerializeField] private VisualTreeAsset upgradeColumn;
    [SerializeField] private VisualTreeAsset upgradeOption;
    
    private VisualElement _root;
    private TextElement _unitName;
    private Camera _unitCamera;
    private List<VisualElement> _tabs;
    private VisualElement _content;
    private Label _description;

    private void Awake() {
      _root = GetComponent<UIDocument>().rootVisualElement;
      _unitCamera = GetComponentInChildren<Camera>();
      SetVisible(false);
    }

    private void OnEnable() {
      Dispatch.ShipBuilder.OpenCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.CloseCharacterSheet.RegisterListener(OnCloseCharacterSheet);
      Dispatch.ShipBuilder.EquipmentUpgradePurchased.RegisterListener(OnPurchase);
    }

    private void OnDisable() {
      Dispatch.ShipBuilder.OpenCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.CloseCharacterSheet.UnregisterListener(OnCloseCharacterSheet);
      Dispatch.ShipBuilder.EquipmentUpgradePurchased.UnregisterListener(OnPurchase);
    }

    private void Start() {
      _unitName = _root.Q<TextElement>("ChampionName");
      _tabs = _root.Query<VisualElement>(className: "panel-tab").ToList();
      for (int i = 0; i < _tabs.Count; i++) {
        var tabIndex = i;
        _tabs[i].RegisterCallback<ClickEvent>(_ => SetActiveTab(tabIndex));
      }

      _content = _root.Q<VisualElement>("UpgradeOptionsContainer");
      _description = _root.Q<Label>("UpgradeDescription");

      SetupResourcesDisplay();
    }

    private void OnPurchase() {
      SetupResourcesDisplay();
    }

    private void SetupResourcesDisplay() {
      var resources = _root.Q<Label>("ResourcesText");
      var inventory = GameState.State.player.inventory;
      resources.text = string.Join(
          "\n",
          SoulTypes.Instance.All().Select(
              soulType => $"{soulType.spriteTag} {inventory.GetQuantity(soulType)} {soulType.displayName}"));
    }

    private void OnOpenCharacterSheet(EncounterActor unit) {
      _unitName.text = unit.EncounterState.metadata.GetName();
      _unitCamera.transform.position = unit.transform.position + new Vector3(0, 0.25f, -10f);
      SetActiveTab(0);
      SetVisible(true);
    }

    private void OnCloseCharacterSheet() {
      SetVisible(false);
    }

    private void SetActiveTab(int tabIndex) {
      if (tabIndex < 0 || tabIndex > _tabs.Count) {
        Debug.LogWarning($"Attempted to activate tab outside tab range: {tabIndex}");
        return;
      }

      for (int i = 0; i < _tabs.Count; i++) {
        var tab = _tabs[i];
        if (i == tabIndex) {
          tab.AddToClassList("panel-tab-active");
          tab.RemoveFromClassList("panel-tab-inactive");
        } else {
          tab.RemoveFromClassList("panel-tab-active");
          tab.AddToClassList("panel-tab-inactive");
        }
      }

      SetContent(GetSlotForTab(tabIndex));
    }

    private void SetContent(EquipmentSlot slot) {
      _content.Clear();
      if (!CurrentSelection.Instance.TryGetUnit<PlayerUnitController>(out var unit)
          || !unit.Metadata.equipped.TryGetValue(slot, out var equipment)) {
        Debug.LogWarning("No selected unit, cannot display upgrade content");
        return;
      }

      foreach (var upgrade in equipment.item.GetAvailableUpgrades()) {
        var column = upgradeColumn.CloneTree();
        _content.Add(column);
        var columnElement = column.Q<VisualElement>("UpgradeColumn");
        
        // We assume upgrade options are only the max tier;
        var currentUpgrade = Option.Some(upgrade);
        while (currentUpgrade.TryGet(out var innerUpgrade)) {
          var option = upgradeOption.CloneTree();
          columnElement.Add(option);
          option.userData = new UpgradeOption(option, innerUpgrade, equipment, _description);
          
          currentUpgrade = innerUpgrade.GetPrerequisite();
        }
      }
    }

    private EquipmentSlot GetSlotForTab(int tabIndex) {
      if (tabIndex == 2) {
        return EquipmentSlots.Instance.utilitySlot;
      }
      
      if (tabIndex == 1) {
        return EquipmentSlots.Instance.apparelSlot;
      }
      
      return EquipmentSlots.Instance.weaponSlot;
    }

    private void SetVisible(bool visible) {
      _root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
  }
}