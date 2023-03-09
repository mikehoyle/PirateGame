using Common.Loading;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Overworld {
  public class ShipManagementButton : MonoBehaviour {
    private Button _button;
    private PreloadedScene _shipScene;

    private void Awake() {
      _button = GetComponent<Button>();
    }

    private void Start() {
      _shipScene = SceneLoader.Instance.PreloadScene(Scenes.Name.ShipBuilder);
    }

    private void OnEnable() {
      _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable() {
      _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick() {
      _shipScene.Activate();
    }
  }
}