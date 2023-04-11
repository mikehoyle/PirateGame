using UnityEngine;
using UnityEngine.UIElements;

namespace Units.UI {
  public class UnitUpgradeDisplay : MonoBehaviour {
    private VisualElement _root;
    
    private void Awake() {
      _root = GetComponent<UIDocument>().rootVisualElement;
    }

    private void Start() {
      var portrait = _root.Q<VisualElement>("Portrait");
    }
  }
}