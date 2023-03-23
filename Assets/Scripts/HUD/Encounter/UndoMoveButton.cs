using Events;
using RuntimeVars.Encounters;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class UndoMoveButton : MonoBehaviour {
    [SerializeField] private CurrentSelection currentSelection;
    
    private Button _button;
    private bool _isPlayerTurn;

    private void Awake() {
      _button = GetComponentInChildren<Button>();
      _button.interactable = false;
      _button.onClick.AddListener(OnButtonClick);
      _isPlayerTurn = false;
    }

    private void OnEnable() {
      Dispatch.Encounters.PlayerTurnStart.RegisterListener(OnPlayerTurnStart);
      Dispatch.Encounters.PlayerTurnEnd.RegisterListener(OnPlayerTurnEnd);
    }

    private void OnDisable() {
      Dispatch.Encounters.PlayerTurnStart.UnregisterListener(OnPlayerTurnStart);
      Dispatch.Encounters.PlayerTurnEnd.UnregisterListener(OnPlayerTurnEnd);
    }

    private void Update() {
      if (_isPlayerTurn && currentSelection.TryGetUnit<PlayerUnitController>(out var unit)) {
        _button.gameObject.SetActive(true);
        _button.interactable = unit.CanUndoMove();
      } else {
        _button.gameObject.SetActive(false);
      }
    }

    private void OnButtonClick() {
      if (currentSelection.TryGetUnit<PlayerUnitController>(out var unit)) {
        unit.UndoMoveIfEligible();
        return;
      }
      
      Debug.LogWarning("Undo button should not be interactable, ignoring click");
    }

    private void OnPlayerTurnStart() {
      _isPlayerTurn = true;
    }
    
    private void OnPlayerTurnEnd() {
      _isPlayerTurn = false;
    }
  }
}