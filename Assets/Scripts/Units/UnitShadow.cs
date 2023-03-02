using FunkyCode;
using UnityEngine;

namespace Units {
  public class UnitShadow : MonoBehaviour {

    private DayLightCollider2D[] _shadowColliders;
    
    private void Awake() {
      _shadowColliders = GetComponents<DayLightCollider2D>();
    }

    public void SetEnabled(bool shadowsEnabled) {
      foreach (var shadowCollider in _shadowColliders) {
        shadowCollider.enabled = shadowsEnabled;
      }
    }
  }
}