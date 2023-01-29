using UnityEngine;

namespace CameraControl {
  public class CameraController : MonoBehaviour {
    public float moveTime;
    
    private bool _isPointFocused;
    private Vector3 _focusPoint;
    private Vector3 _velocity = Vector3.zero;

    void LateUpdate() {
      if (_isPointFocused) {
        if (transform.position != _focusPoint) {
          transform.position = Vector3.SmoothDamp(transform.position, _focusPoint, ref _velocity, moveTime);
        }
      }
    }

    public void SetFocusPoint(Vector3 focus) {
      _isPointFocused = true;
      _focusPoint = new Vector3(focus.x, focus.y, transform.position.z);
    }

    public void SnapToPoint(Vector3 focus) {
      _isPointFocused = false;
      transform.position = new Vector3(focus.x, focus.y, transform.position.z);
    }
  }
}