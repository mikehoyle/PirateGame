using Encounters;
using Events;
using Units.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class AvailableAction : MonoBehaviour {
    private Text _costField;
    private Text _hotkeyField;
    private Text _descriptionField;
    private Text _damageField;
    private Button _button;
    private int _abilityIndex;
    private Text _remainingUsesField;

    private void Awake() {
      _costField = transform.Find("Cost").GetComponent<Text>();
      _remainingUsesField = transform.Find("RemainingUses").GetComponent<Text>();
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

    public void Init(int abilityIndex, UnitAbility ability, EncounterActor actor) {
      _costField.text = ability.CostString();
      _abilityIndex = abilityIndex;
      _hotkeyField.text = abilityIndex.ToString();
      _descriptionField.text = ability.displayString;
      _damageField.text = ability.descriptionShort;

      if (ability.usesPerEncounter <= 0) {
        _remainingUsesField.text = "";
      } else {
        _remainingUsesField.text = $"{ability.GetRemainingUses(actor)} / {ability.usesPerEncounter}";
      }
    }

    private void OnButtonClick() {
      Dispatch.Encounters.TrySelectAbilityByIndex.Raise(_abilityIndex - 1);
    }
  }
}