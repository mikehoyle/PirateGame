using System.Net.Mime;
using RuntimeVars.Encounters.Events;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class AvailableAction : MonoBehaviour {
    [SerializeField] private EncounterEvents encounterEvents;
    
    private Text _costField;
    private Text _hotkeyField;
    private Text _descriptionField;
    private Text _damageField;
    private Button _button;
    private int _abilityIndex;

    private void Awake() {
      _costField = transform.Find("Cost").GetComponent<Text>();
      _hotkeyField = transform.Find("Hotkey").GetComponent<Text>();
      _descriptionField = transform.Find("Description").GetComponent<Text>();
      _damageField = transform.Find("Damage").GetComponent<Text>();
      
      _button = GetComponent<Button>();
    }

    private void OnEnable() {
      _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable() {
      _button.onClick.RemoveListener(OnButtonClick);
    }

    public void SetUnavailable() {
      _button.interactable = false;
    }

    public void Init(string cost, int abilityIndex, string actionDescription, string effectDescription) {
      _costField.text = cost;
      _abilityIndex = abilityIndex;
      _hotkeyField.text = abilityIndex.ToString();
      _descriptionField.text = actionDescription;
      _damageField.text = effectDescription;
    }

    private void OnButtonClick() {
      encounterEvents.trySelectAbilityByIndex.Raise(_abilityIndex - 1);
    }
  }
}