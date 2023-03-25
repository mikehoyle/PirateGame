﻿using Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Encounters {
  public abstract class EncounterInputReceiver : MonoBehaviour, GameControls.ITurnBasedEncounterActions {

    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      OnClick(Mouse.current.position.ReadValue());
    }

    public void OnSelectActionOne(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      OnTrySelectAction(0);
    }
    
    public void OnSelectActionTwo(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      OnTrySelectAction(1);
    }
    
    public void OnSelectActionThree(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      OnTrySelectAction(2);
    }
    
    public void OnSelectActionFour(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      OnTrySelectAction(3);
    }
    
    public void OnSelectActionFive(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      OnTrySelectAction(4);
    }
    
    public void OnEndTurn(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      OnEndTurnKey();
    }

    public void OnCancelSelection(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      OnCancelSelection();
    }

    protected abstract void OnTrySelectAction(int index);
    protected abstract void OnClick(Vector2 mouseLocation);
    protected abstract void OnEndTurnKey();
    protected abstract void OnCancelSelection();
  }
}