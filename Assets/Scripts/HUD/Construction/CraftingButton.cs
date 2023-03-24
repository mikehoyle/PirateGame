using Events;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  public class CraftingButton : MonoBehaviour{
    private Button _button;
    
    private void Awake() {
      _button = GetComponent<Button>();
      _button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
      Dispatch.ShipBuilder.OpenCraftingMenu.Raise();
    }
  }
}