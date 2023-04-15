using Common.Loading;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Overworld {
  public class ShipManagementButton : MonoBehaviour {
    private Button _button;

    private void Awake() {
      _button = GetComponent<Button>();
    }

    private void OnEnable() {
      _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable() {
      _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick() {
      SceneLoader.Instance.LoadScene(SceneId.ShipBuilder);
    }
  }
}