using System;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class AvailableAction : MonoBehaviour {
    private Text _hotkeyField;
    private Text _descriptionField;
    private Button _button;

    private void Awake() {
      _hotkeyField = transform.Find("Hotkey").GetComponent<Text>();
      _descriptionField = transform.Find("Description").GetComponent<Text>();
      
      // TODO(P1): If these stay buttons, make them actually work
      _button = GetComponent<Button>();
    }

    public void SetUnavailable() {
      _button.interactable = false;
    }

    public void Init(string hotkey, string actionDescription) {
      _hotkeyField.text = hotkey;
      _descriptionField.text = actionDescription;
    }
  }
}