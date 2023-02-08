using System;
using System.Collections.Generic;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using State;
using State.Unit;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class ActionMenuController : MonoBehaviour {
    [SerializeField] private GameObject availableActionPrefab;
    [SerializeField] private UnitSelectedEvent unitSelectedEvent; 
    
    private HorizontalLayoutGroup _container;
    
    private void Awake() {
      _container = GetComponentInChildren<HorizontalLayoutGroup>();
    }

    private void OnEnable() {
      unitSelectedEvent.RegisterListener(RefreshActionMenu);
    }

    private void OnDisable() {
      unitSelectedEvent.UnregisterListener(RefreshActionMenu);
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