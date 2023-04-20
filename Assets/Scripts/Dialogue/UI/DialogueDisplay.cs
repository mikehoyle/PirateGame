﻿using System;
using System.Collections.Generic;
using Common;
using Controls;
using Events;
using RuntimeVars;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dialogue.UI {
  public class DialogueDisplay : MonoBehaviour, GameControls.IPressAnyKeyActions { 

    private IEnumerator<DialogueSegment> _sequence;
    private List<DialogueSpeaker> _speakers;
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
      GameInput.Controls.PressAnyKey.SetCallbacks(this);
    }

    private void OnDisable() {
      GameInput.Controls.PressAnyKey.RemoveCallbacks(this);
    }

    public void Initialize(string dialogueJson, List<DialogueSpeaker> speakers) {
      _sequence = DialogueSequence.ParseFrom(dialogueJson).Each();
      _speakers = speakers;
      DisplayNext();
      GameInput.AddInputBlocker(this);
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
        GameInput.RemoveInputBlocker(this);
        Destroy(gameObject);
        return;
      }

      NewContent?.Invoke(_sequence.Current, _speakers);
    }
  }
}


