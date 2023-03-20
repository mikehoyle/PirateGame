using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue.UI {
  public class ContinueDisplay : MonoBehaviour {
    private Text _text;
    
    private void Awake() {
      _text = GetComponent<Text>();
      GetComponentInParent<DialogueDisplay>().NewContent += OnNewContent;
    }

    private void OnNewContent(DialogueSegment segment, List<DialogueSpeaker> speakers) {
      if (string.IsNullOrEmpty(segment.ContinuePrompt)) {
        _text.text = "[ Press Any Key To Continue ]";
        return;
      }
      _text.text = $"[ {segment.ContinuePrompt} ]";
    }
  }
}