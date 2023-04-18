using System.Linq;
using Events;
using StaticConfig.Equipment;
using StaticConfig.Equipment.Upgrades;
using UnityEngine.UIElements;

namespace Units.UI {
  public class UpgradeOption {
    private readonly VisualElement _upgradeOption;
    private readonly VisualElement _icon;
    private readonly EquipmentUpgrade _upgrade;
    private readonly EquipmentItemInstance _item;
    private readonly Label _cost;
    private readonly Label _descriptionField;

    public UpgradeOption(
        VisualElement upgradeOption, EquipmentUpgrade upgrade, EquipmentItemInstance item, Label descriptionField) {
      _upgradeOption = upgradeOption.Q("UpgradeOption");
      _icon = upgradeOption.Q("UpgradeIcon");
      _cost = upgradeOption.Q<Label>("UpgradeCost");
      _upgrade = upgrade;
      _item = item;
      _descriptionField = descriptionField;

      _upgradeOption.RegisterCallback<ClickEvent>(OnClick);
      _upgradeOption.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
      _upgradeOption.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
      _upgradeOption.RegisterCallback<DetachFromPanelEvent>(OnDestroy);
      SetStyle();
      Dispatch.ShipBuilder.EquipmentUpgradePurchased.RegisterListener(OnUpgradePurchased);
    }

    private void OnDestroy(DetachFromPanelEvent evt) {
      Dispatch.ShipBuilder.EquipmentUpgradePurchased.UnregisterListener(OnUpgradePurchased);
    }
    
    private void OnUpgradePurchased() {
      // When any upgrade is purchased, recalculate the style.
      SetStyle();
    }

    private void SetStyle() {
      _upgradeOption.ClearClassList();
      _upgradeOption.AddToClassList("upgrade");
      _cost.text = GetCostString();
      
      var upgradeState = _item.GetUpgradeState(_upgrade);
      switch (upgradeState) {
        default:
        case UpgradeState.Locked:
          _upgradeOption.AddToClassList("upgrade-unavailable");
          break;
        case UpgradeState.Available:
          _upgradeOption.AddToClassList("upgrade-available");
          break;
        case UpgradeState.AvailableButUnaffordable:
          _upgradeOption.AddToClassList("upgrade-unaffordable");
          break;
        case UpgradeState.Acquired:
          _upgradeOption.AddToClassList("upgrade-acquired");
          break;
      }

      if (upgradeState != UpgradeState.Locked) {
        _icon.style.backgroundImage = Background.FromSprite(_upgrade.icon);
      }
    }
    private string GetCostString() {
      return string.Join(
          "\n", _upgrade.cost.Select(lineItem => $"{lineItem.cost} {lineItem.resource.spriteTag}"));
    }

    private void OnClick(ClickEvent clickEvent) {
      if (clickEvent.button != 0) {
        return;
      }
      
      _item.AttemptPurchaseUpgrade(_upgrade);
    }
    
    private void OnMouseEnter(MouseEnterEvent evt) {
      _descriptionField.text =
          $"<line-height=150%><b>{_upgrade.displayName}</b>\n{_upgrade.longDescription}</line-height>";
    }
    
    private void OnMouseLeave(MouseLeaveEvent evt) {
      _descriptionField.text = "";
    }
  }
}