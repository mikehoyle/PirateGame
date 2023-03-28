using System;
using Encounters;
using Events;
using RuntimeVars.Encounters;
using Units;
using Units.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class AvailableAction : MonoBehaviour {
    private UnitAbility _ability;
    private Text _costField;
    private Text _hotkeyField;
    private Text _descriptionField;
    private Text _damageField;
    private Button _button;
    private int _abilityIndex;
    private Text _remainingUsesField;
    private bool disabled;

    private void Awake() {
      _costField = transform.Find("Cost").GetComponent<Text>();
      _remainingUsesField = transform.Find("RemainingUses").GetComponent<Text>();
      _hotkeyField = transform.Find("Hotkey").GetComponent<Text>();
      _descriptionField = transform.Find("Description").GetComponent<Text>();
      _damageField = transform.Find("Damage").GetComponent<Text>();
      
      _button = GetComponent<Button>();
    }

    private void OnEnable() {
      Dispatch.Encounters.AbilityExecutionStart.RegisterListener(OnAbilityExecutionStart);
      Dispatch.Encounters.AbilityExecutionEnd.RegisterListener(OnAbilityExecutionEnd);
      _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable() {
      Dispatch.Encounters.AbilityExecutionStart.UnregisterListener(OnAbilityExecutionStart);
      Dispatch.Encounters.AbilityExecutionEnd.UnregisterListener(OnAbilityExecutionEnd);
      _button.onClick.RemoveListener(OnButtonClick);
    }

    private void Update() {
      SetAvailable(false);
      if (disabled || !CurrentSelection.Instance.TryGetUnit<PlayerUnitController>(out var unit)) {
        return;
      }

      if (_ability != null && _ability.CanAfford(unit)) {
        SetAvailable(true);
      }
    }

    private void SetAvailable(bool isAvailable) {
      _button.interactable = isAvailable;
    }

    public void Init(int abilityIndex, UnitAbility ability, EncounterActor actor) {
      _ability = ability;
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

    private void OnAbilityExecutionStart(EncounterActor param1, UnitAbility param2) {
      disabled = true;
    }
    
    private void OnAbilityExecutionEnd(EncounterActor param1, UnitAbility param2) {
      disabled = false;
    }

    private void OnButtonClick() {
      Dispatch.Encounters.TrySelectAbilityByIndex.Raise(_abilityIndex - 1);
    }
  }
}