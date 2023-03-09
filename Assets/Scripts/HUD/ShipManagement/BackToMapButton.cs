using Common.Loading;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement {
  public class BackToMapButton : MonoBehaviour {
    private Button _button;
    private PreloadedScene _mapScene;

    private void Awake() {
      _button = GetComponent<Button>();
    }

    private void Start() {
      _mapScene = SceneLoader.Instance.PreloadScene(Scenes.Name.Overworld);
    }

    private void OnEnable() {
      _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable() {
      _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick() {
      _mapScene.Activate();
    }
  }
}