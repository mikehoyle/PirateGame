using System;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class ActionMenuController : MonoBehaviour {
    [SerializeField] private GameObject availableActionPrefab;
    
    private HorizontalLayoutGroup _container;
    private void Awake() {
      _container = GetComponentInChildren<HorizontalLayoutGroup>();
    }

    public void DisplayMenuItemsForUnit(UnitController unit) {
      Clear();
      var currentHotkey = 1;
      foreach (var capableAction in unit.EncounterMetadata.CapableActions) {
        var item = Instantiate(availableActionPrefab, _container.transform).GetComponent<AvailableAction>();
        item.Init(Convert.ToString(currentHotkey), capableAction.DisplayString());
        if (!unit.EncounterMetadata.AvailableActions.Contains(capableAction)) {
          item.GetComponent<Button>().interactable = false;
        }
        currentHotkey++;
      }
    }

    private void Clear() {
      foreach (Transform child in _container.transform) {
        Destroy(child.gameObject);
      }
    }

    public static ActionMenuController Get() {
      return GameObject.FindWithTag(Tags.EncounterActionDisplay).GetComponent<ActionMenuController>();
    }
  }
}