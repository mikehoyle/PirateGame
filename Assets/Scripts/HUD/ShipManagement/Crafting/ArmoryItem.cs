using Events;
using StaticConfig.Equipment;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement.Crafting {
  public class ArmoryItem : MonoBehaviour {
    
    private Text _text;
    private Button _button;
    private EquipmentItemInstance _item;

    private void Awake() {
      _text = GetComponentInChildren<Text>();
      _button = GetComponent<Button>();
      _button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton() {
      Dispatch.ShipBuilder.AttemptEquipItem.Raise(_item);
    }

    public void Initialize(EquipmentItemInstance item) {
      _item = item;
      _text.text = item.item.DisplayDescription();
    }
  }
}