﻿using Encounters;
using Events;
using State.Unit;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class ActionMenuController : MonoBehaviour {
    [SerializeField] private GameObject availableActionPrefab;
    
    private Canvas _canvas;
    private HorizontalLayoutGroup _container;

    private void Awake() {
      _canvas = GetComponent<Canvas>();
      _container = GetComponentInChildren<HorizontalLayoutGroup>();
      SetVisible(false);
    }

    private void OnEnable() {
      Dispatch.Encounters.UnitSelected.RegisterListener(RefreshActionMenu);
      Dispatch.Encounters.PlayerTurnEnd.RegisterListener(OnPlayerTurnEnd);
    }

    private void OnDisable() {
      Dispatch.Encounters.UnitSelected.UnregisterListener(RefreshActionMenu);
      Dispatch.Encounters.PlayerTurnEnd.UnregisterListener(OnPlayerTurnEnd);
    }

    private void OnPlayerTurnEnd() {
      SetVisible(false);
    }

    private void RefreshActionMenu(EncounterActor unit) {
      Clear();
      if (unit == null || unit.EncounterState.faction != UnitFaction.PlayerParty) {
        SetVisible(false);
        return;
      }
      
      SetVisible(true);
      var currentHotkey = 1;
      foreach (var capableAction in unit.GetAllCapableAbilities()) {
        var item = Instantiate(availableActionPrefab, _container.transform).GetComponent<AvailableAction>();
        item.Init(currentHotkey, capableAction, unit);
        currentHotkey++;
      }
    }

    private void SetVisible(bool isVisible) {
      _canvas.enabled = isVisible;
    }

    private void Clear() {
      foreach (Transform child in _container.transform) {
        Destroy(child.gameObject);
      }
    }
  }
}