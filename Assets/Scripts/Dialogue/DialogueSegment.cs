namespace Dialogue {
  /// <summary>
  /// Represents a single segment of dialogue, parsed verbatim from json.
  /// </summary>
  public struct DialogueSegment {
    public int SpeakerId;
    public string Content;
    public string ContinuePrompt;
  }
}