using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue.UI {
  public class ContentDisplay : MonoBehaviour {
    private Text _text;
    
    private void Awake() {
      _text = GetComponent<Text>();
      GetComponentInParent<DialogueDisplay>().NewContent += OnNewContent;
    }

    private void OnNewContent(DialogueSegment segment, List<DialogueSpeaker> speakers) {
      _text.text = segment.Content;
    }
  }
}