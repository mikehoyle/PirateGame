using Common.Events;

namespace Events {
  public class CommonEvents {
    public readonly GameEvent DialogueStart = new();
    public readonly GameEvent DialogueEnd = new();
  }
}