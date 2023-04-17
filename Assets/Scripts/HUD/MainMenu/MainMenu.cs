using System;
using Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace HUD.MainMenu {
  public class MainMenu : MonoBehaviour, GameControls.IMenuActions {
    private VisualElement _root;
    private VisualElement _content;
    private GameControls _controls;

    private void Awake() {
      _root = GetComponent<UIDocument>().rootVisualElement;
      _root.style.display = DisplayStyle.None;
      _content = _root.Q<VisualElement>("MenuContent");
    }

    private void Start() {
      _root.Q<Label>("MenuTitle").text = "Main Menu";
      AddButton("Exit Game", OnExitGame);
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.Menu.SetCallbacks(this);
      }
      _controls.Menu.Enable();
    }

    private void OnDisable() {
      _controls.Menu.Disable();
    }

    private void AddButton(string buttonName, Action onClick) {
      var button = new Button(onClick) {
          text = buttonName,
      };
      button.AddToClassList("menuButton");
      _content.Add(button);
    }

    private void OnExitGame() {
      Debug.Log("Exiting game");
      Application.Quit(0);
    }

    public void OnToggleMenu(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      
      _root.style.display = 
          _root.style.display.value == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
    }
  }
}