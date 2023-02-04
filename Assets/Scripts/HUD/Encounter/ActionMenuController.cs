using System;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class ActionMenuController : MonoBehaviour {
    [SerializeField] private GameObject availableActionPrefab;
    
    private HorizontalLayoutGroup _container;
    private UnitEncounterManager _activeUnit;
    private List<AvailableAction> _displayedItems;
    
    private void Awake() {
      _container = GetComponentInChildren<HorizontalLayoutGroup>();
      _displayedItems = new();
    }

    private void Update() {
      if (_activeUnit == null) {
        Clear();
        return;
      }
      
      for (int i = 0; i < _activeUnit.CapableActions.Count; i++) {
        if (!_activeUnit.AvailableActions.Contains(_activeUnit.CapableActions[i])) {
          _displayedItems[i].SetUnavailable();
        }
      }
    }

    public void SetActiveUnit(UnitEncounterManager unit) {
      _activeUnit = unit;
      Clear();
      var currentHotkey = 1;
      foreach (var capableAction in unit.CapableActions) {
        var item = Instantiate(availableActionPrefab, _container.transform).GetComponent<AvailableAction>();
        item.Init(Convert.ToString(currentHotkey), capableAction.DisplayString());
        _displayedItems.Add(item);
        currentHotkey++;
      }
    }

    private void Clear() {
      _displayedItems = new();
      foreach (Transform child in _container.transform) {
        Destroy(child.gameObject);
      }
    }

    public static ActionMenuController Get() {
      return GameObject.FindWithTag(Tags.EncounterActionDisplay).GetComponent<ActionMenuController>();
    }
  }
}