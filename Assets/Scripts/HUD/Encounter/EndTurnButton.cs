using System.Collections;
using Encounters;
using Events;
using RuntimeVars;
using StaticConfig.Units;
using Units.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class EndTurnButton : MonoBehaviour {
    [SerializeField] private UnitCollection unitsInEncounter;
    
    private Button _button;

    private void Awake() {
      _button = GetComponentInChildren<Button>();
      _button.onClick.AddListener(OnButtonClick);
      _button.gameObject.SetActive(false);
    }

    private void OnEnable() {
      Dispatch.Encounters.PlayerTurnStart.RegisterListener(OnPlayerTurnStart);
      Dispatch.Encounters.PlayerTurnEnd.RegisterListener(OnPlayerTurnEnd);
      Dispatch.Encounters.AbilityExecutionEnd.RegisterListener(OnAbilityExecutionEnd);
    }

    private void OnDisable() {
      Dispatch.Encounters.PlayerTurnStart.UnregisterListener(OnPlayerTurnStart);
      Dispatch.Encounters.PlayerTurnEnd.UnregisterListener(OnPlayerTurnEnd);
      Dispatch.Encounters.AbilityExecutionEnd.UnregisterListener(OnAbilityExecutionEnd);
    }
    
    private void OnButtonClick() {
      Dispatch.Encounters.PlayerTurnEndRequest.Raise();
    }

    private void OnPlayerTurnStart() {
      _button.gameObject.SetActive(true);
    }
    
    private void OnPlayerTurnEnd() {
      _button.gameObject.SetActive(false);
    }

    private void OnAbilityExecutionEnd(EncounterActor actor, UnitAbility ability) {
      var playerStillHasActions = false;
      foreach (var unit in unitsInEncounter) {
        if (unit.EncounterState.GetResourceAmount(ExhaustibleResources.Instance.ap) > 0) {
          playerStillHasActions = true;
        } 
      }

      if (!playerStillHasActions) {
        StartCoroutine(FlashButton());
      }
    }

    private IEnumerator FlashButton() {
      var startingColor = _button.colors.normalColor;
      var flashColor = Color.green;

      var colors = _button.colors;
      for (int i = 0; i < 6; i++) {
        colors.normalColor = i % 2 == 0 ? flashColor : startingColor;
        _button.colors = colors;
        yield return new WaitForSeconds(0.5f);
      }
      colors.normalColor = startingColor;
      _button.colors = colors;
    }
  }
}