using System;
using System.Collections.Generic;
using Controls;
using RuntimeVars;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dialogue.UI {
  public class DialogueDisplay : MonoBehaviour, GameControls.IPressAnyKeyActions {
    [SerializeField] private CommonEvents commonEvents; 

    private IEnumerator<DialogueSegment> _sequence;
    private List<DialogueSpeaker> _speakers;
    private GameControls _controls;
    private Canvas _canvas;

    public delegate void OnNewDialogueContent(DialogueSegment segment, List<DialogueSpeaker> speakers);
    public event OnNewDialogueContent NewContent;

    private void Awake() {
      // Disable until initialized
      _canvas = GetComponent<Canvas>();
      _canvas.enabled = false;
      enabled = false;
    }

    private void OnDestroy() {
      NewContent = null;
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls ??= new GameControls();
        _controls.PressAnyKey.SetCallbacks(this);
      }

      _controls.PressAnyKey.Enable();
    }

    private void OnDisable() {
      _controls?.PressAnyKey.Disable();
    }

    public void Initialize(string dialogueJson, List<DialogueSpeaker> speakers) {
      _sequence = DialogueSequence.ParseFrom(dialogueJson).Each();
      _speakers = speakers;
      DisplayNext();
      commonEvents.dialogueStart.Raise();
      enabled = true;
      _canvas.enabled = true;
    }

    public void OnAnyKey(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      DisplayNext();
    }

    private void DisplayNext() {
      if (!_sequence.MoveNext()) {
        commonEvents.dialogueEnd.Raise();
        Destroy(gameObject);
        return;
      }

      NewContent?.Invoke(_sequence.Current, _speakers);
    }
  }
}


