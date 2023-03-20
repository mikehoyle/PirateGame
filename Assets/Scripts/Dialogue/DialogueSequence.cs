using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dialogue {
  /// <summary>
  /// A class intended to be deserialized from json to represent a simple linear
  /// sequence of dialogue.
  /// </summary>
  public class DialogueSequence {
    private List<DialogueSegment> _segments;

    private DialogueSequence() { }

    public static DialogueSequence ParseFrom(string json) {
      var result = new DialogueSequence {
          _segments = JsonConvert.DeserializeObject<List<DialogueSegment>>(json),
      };

      return result;
    }

    public IEnumerator<DialogueSegment> Each() {
      return _segments.GetEnumerator();
    }
  }
}