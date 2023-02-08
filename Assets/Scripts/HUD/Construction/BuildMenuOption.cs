using System;
using State;
using StaticConfig;
using StaticConfig.Builds;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  // TODO(P1): Revamp this UI entirely
  public class BuildMenuOption : MonoBehaviour {
    private Text _nameField;
    private Text _costField;
    private ConstructableObject _constructable;
    private Button _button;
    private InventoryState _inventoryState;

    public event EventHandler<ConstructableObject> OnBuildOptionSelected;

    private void Awake() {
      _inventoryState = GameState.State.player.inventory;
      _nameField = transform.Find("Name").GetComponent<Text>();
      _costField = transform.Find("Cost").GetComponent<Text>();

      _button = GetComponent<Button>();
      var onClick = new Button.ButtonClickedEvent();
      onClick.AddListener(OnClick);
      _button.onClick = onClick;
    }

    private void Update() {
      UpdateCostLine();
    }

    public void Init(ConstructableObject constructable) {
      _constructable = constructable; 
      _nameField.text = constructable.buildDisplayName;
      UpdateCostLine();
    }
    
    private void UpdateCostLine() {
      _button.interactable = true;
      _costField.color = Color.black;
      _costField.text = "";
      foreach (var lineItem in _constructable.buildCost) {
        var inventoryQuantity = _inventoryState.GetQuantity(lineItem.resource);
        _costField.text += $"{lineItem.resource.displayName} " +
            $"{inventoryQuantity}/{lineItem.cost}\n";
        if (inventoryQuantity < lineItem.cost) {
          _costField.color = new Color(0.57f, 0f, 0f);
          _button.interactable = false;
        }
      }
    }

    private void OnClick() {
      if (_button.interactable) {
        OnBuildOptionSelected?.Invoke(this, _constructable);
      }
    }
  }
}