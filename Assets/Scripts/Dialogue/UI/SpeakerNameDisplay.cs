using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue.UI {
  public class SpeakerNameDisplay : MonoBehaviour {
    private Text _text;
    
    private void Awake() {
      _text = GetComponent<Text>();
      GetComponentInParent<DialogueDisplay>().NewContent += OnNewContent;
    }

    private void OnNewContent(DialogueSegment segment, List<DialogueSpeaker> speakers) {
      if (segment.SpeakerId < 0 || segment.SpeakerId >= speakers.Count) {
        _text.text = "";
        return;
      }
      _text.text = speakers[segment.SpeakerId].Name;
    }
  }
}