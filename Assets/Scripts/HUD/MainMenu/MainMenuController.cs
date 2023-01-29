using UnityEngine;
using UnityEngine.UI;

namespace HUD.MainMenu {
  public class MainMenuController : MonoBehaviour {
    [SerializeField] private GameObject _buttonPrefab;
    private Transform _container;

    private void Awake() {
      _container = transform.GetChild(0);
    }

    public void AddMenuItem(string label, Button.ButtonClickedEvent onClick) {
      var button = Instantiate(_buttonPrefab, _container).GetComponent<Button>();
      button.onClick = onClick;
      button.GetComponentInChildren<Text>().text = label;
    }

    public static MainMenuController Get() {
      return GameObject.FindGameObjectWithTag(Tags.MainMenu).GetComponent<MainMenuController>();
    }
  }
}