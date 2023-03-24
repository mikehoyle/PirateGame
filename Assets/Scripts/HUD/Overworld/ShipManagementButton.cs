using Common.Loading;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Overworld {
  public class ShipManagementButton : MonoBehaviour {
    private Button _button;
    private PreloadedScene _shipBuilderScene;

    private void Awake() {
      _button = GetComponent<Button>();
    }

    private void Start() {
      _shipBuilderScene = SceneLoader.Instance.PreloadScene(Scenes.Name.ShipBuilder);
    }

    private void OnEnable() {
      _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable() {
      _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick() {
      _shipBuilderScene.Activate();
    }
  }
}