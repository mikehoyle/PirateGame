using System.Collections.Generic;
using Controls;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace HUD.MainMenu {
  public class MainMenuController : MonoBehaviour, GameControls.IMenuActions {
    [SerializeField] private GameObject _buttonPrefab;
    private Transform _container;
    private Canvas _canvas;
    private readonly Dictionary<string, Button> _buttons = new();
    private GameControls _controls;

    private void Awake() {
      _container = transform.GetChild(0);
      _canvas = GetComponent<Canvas>();
      AddMenuItem("Exit Game", OnExitGame);
      Hide();
    }

    private void OnEnable() {
      _canvas.worldCamera = Camera.main;
      _controls ??= new GameControls();
      _controls.Menu.SetCallbacks(this);
      _controls.Menu.Enable();
    }

    private void OnDisable() {
      _controls.Menu.Disable();
    }

    private void Hide() {
      _canvas.enabled = false;
    }

    private void Toggle() {
      _canvas.enabled = !_canvas.enabled;
    }

    public void AddMenuItem(string label, UnityAction onClick) {
      var button = Instantiate(_buttonPrefab, _container).GetComponent<Button>();
      button.transform.SetAsFirstSibling();
      _buttons.Add(label, button);
      var onClickEvent = new Button.ButtonClickedEvent();
      onClickEvent.AddListener(onClick);
      button.onClick = onClickEvent;
      button.GetComponentInChildren<Text>().text = label;
    }

    public void RemoveMenuItem(string label) {
      if (_buttons.TryGetValue(label, out var button)) {
        if (!button.IsDestroyed()) {
          Destroy(button.gameObject); 
        }
        _buttons.Remove(label);
      } else {
        Debug.LogWarning($"No button to remove by name {label}");
      }
    }

    private void OnExitGame() {
      Application.Quit(0);
    }

    public void OnToggleMenu(InputAction.CallbackContext context) {
      Toggle();
    }
    
    public static MainMenuController Get() {
      return GameObject.FindGameObjectWithTag(Tags.MainMenu).GetComponent<MainMenuController>();
    }
  }
}