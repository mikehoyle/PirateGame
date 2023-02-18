using System;
using RuntimeVars.Encounters.Events;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class ActionMenuController : MonoBehaviour {
    [SerializeField] private GameObject availableActionPrefab;
    [SerializeField] private EncounterEvents encounterEvents;
    
    private Canvas _canvas;
    private HorizontalLayoutGroup _container;

    private void Awake() {
      _canvas = GetComponent<Canvas>();
      _container = GetComponentInChildren<HorizontalLayoutGroup>();
      _canvas.enabled = false;
    }

    private void OnEnable() {
      encounterEvents.unitSelected.RegisterListener(RefreshActionMenu);
      encounterEvents.playerTurnStart.RegisterListener(OnPlayerTurnStart);
      encounterEvents.playerTurnEnd.RegisterListener(OnPlayerTurnEnd);
    }

    private void OnDisable() {
      encounterEvents.unitSelected.UnregisterListener(RefreshActionMenu);
      encounterEvents.playerTurnStart.UnregisterListener(OnPlayerTurnStart);
      encounterEvents.playerTurnEnd.UnregisterListener(OnPlayerTurnEnd);
    }

    private void OnPlayerTurnStart() {
      _canvas.enabled = true;
    }

    private void OnPlayerTurnEnd() {
      _canvas.enabled = false;
    }

    private void RefreshActionMenu(UnitController unit) {
      Clear();
      var currentHotkey = 1;
      foreach (var capableAction in unit.GetAllCapableAbilities()) {
        var item = Instantiate(availableActionPrefab, _container.transform).GetComponent<AvailableAction>();
        item.Init(Convert.ToString(currentHotkey), capableAction.displayString);
        currentHotkey++;
      }
    }

    private void Clear() {
      foreach (Transform child in _container.transform) {
        Destroy(child.gameObject);
      }
    }
  }
}