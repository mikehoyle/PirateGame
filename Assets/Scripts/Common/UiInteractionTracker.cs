using UnityEngine;
using UnityEngine.EventSystems;

namespace Common {
  /// <summary>
  /// This unfortunate script is the result of dealing with the annoying way Unity handles
  /// UI interaction.
  /// </summary>
  public class UiInteractionTracker : MonoBehaviour {
    public bool isPlayerHoveringUi;
    
    private EventSystem _eventSystem;

    private void Awake() {
      _eventSystem = EventSystem.current;
    }

    private void Update() {
      isPlayerHoveringUi = _eventSystem.IsPointerOverGameObject();
    }
  }
}