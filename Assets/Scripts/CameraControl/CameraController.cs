using Common;
using MilkShake;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraControl {
  public class CameraController : MonoBehaviour {
    public float moveTime;
    public ShakePreset defaultShake;
    
    private bool _isPointFocused;
    private Vector3 _focusPoint;
    private Vector3 _velocity = Vector3.zero;
    private Camera _camera;
    private Shaker _shaker;

    private void Awake() {
      _camera = Camera.main;
      _shaker = GetComponentInChildren<Shaker>();
    }

    private void LateUpdate() {
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

    public RaycastHit2D RaycastFromMousePosition(LayerMask layerMask) {
      var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
      return Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, layerMask);
    }

    public static CameraController Get() {
      return GameObject.FindWithTag(Tags.MainCameraContainer).GetComponent<CameraController>();
    }
  }
}