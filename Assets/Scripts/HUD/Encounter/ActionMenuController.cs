using System;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class ActionMenuController : MonoBehaviour {
    // TODO(P1): This is absolutely not how actions should be stored and needs to be changed asap.
    private static string[] _allActions = {
        "Move", "Attack",
    };
    [SerializeField] private GameObject availableActionPrefab;
    
    private HorizontalLayoutGroup _container;
    private void Awake() {
      _container = GetComponentInChildren<HorizontalLayoutGroup>();
    }

    private void Start() {
      CreateMenuItems();
    }
    private void CreateMenuItems() {
      for (int i = 0; i < _allActions.Length; i++) {
        var item = Instantiate(availableActionPrefab, _container.transform).GetComponent<AvailableAction>();
        item.Init(Convert.ToString(i + 1), _allActions[i]);
      }
    }
  }
}