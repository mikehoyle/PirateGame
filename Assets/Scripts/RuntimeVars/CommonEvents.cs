using Common.Events;
using UnityEngine;

namespace RuntimeVars {
  [CreateAssetMenu(menuName = "Events/CommonEvents")]
  public class CommonEvents : ScriptableObject {
    public EmptyGameEvent dialogueStart;
    public EmptyGameEvent dialogueEnd;
  }
}