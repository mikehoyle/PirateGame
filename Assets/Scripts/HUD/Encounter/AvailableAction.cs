using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class AvailableAction : MonoBehaviour {
    private Text _costField;
    private Text _hotkeyField;
    private Text _descriptionField;
    private Button _button;

    private void Awake() {
      _costField = transform.Find("Cost").GetComponent<Text>();
      _hotkeyField = transform.Find("Hotkey").GetComponent<Text>();
      _descriptionField = transform.Find("Description").GetComponent<Text>();
      
      // TODO(P1): If these stay buttons, make them actually work
      _button = GetComponent<Button>();
    }

    public void SetUnavailable() {
      _button.interactable = false;
    }

    public void Init(string cost, string hotkey, string actionDescription) {
      _costField.text = cost;
      _hotkeyField.text = hotkey;
      _descriptionField.text = actionDescription;
    }
  }
}