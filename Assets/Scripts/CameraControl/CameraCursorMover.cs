using Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraControl {
  /// <summary>
  /// Controls the camera based on an invisible cursor which the camera follows.
  /// Usage: Add script as component to any object in scene.
  /// 
  /// TODO(P1): Support indicating camera bounds.
  /// TODO(P2): Add an input for snapping back to a point of focus. 
  /// </summary>
  public class CameraCursorMover : MonoBehaviour, GameControls.ICameraCursorMovementActions {
    [SerializeField] private float cameraMoveSpeed;
    
    private Vector3 _cameraCursor;
    private Vector3 _cameraCursorVelocity;
    private CameraController _camera;
    private GameControls _controls;

    private void Awake() {
      _camera = CameraController.Get();
    }
    
    private void OnEnable() {
      if (_controls == null) {
        _controls ??= new GameControls();
        _controls.CameraCursorMovement.SetCallbacks(this);
      }

      _controls.CameraCursorMovement.Enable();
    }

    private void OnDisable() {
      _controls.CameraCursorMovement.Disable();
    }

    private void Update() {
      _cameraCursor += _cameraCursorVelocity * Time.deltaTime;
      _camera.SetFocusPoint(_cameraCursor);
    }

    public void Initialize(Vector3 initialPosition) {
      _cameraCursor = initialPosition;
      _camera.SnapToPoint(_cameraCursor);
    }

    public void MoveCursorDirectly(Vector3 targetPosition) {
      _cameraCursor = targetPosition;
      _cameraCursorVelocity = Vector3.zero;
    }
    
    public void OnMoveCamera(InputAction.CallbackContext context) {
      var normalizedInputVector = context.ReadValue<Vector2>();
      _cameraCursorVelocity = normalizedInputVector * cameraMoveSpeed;
    }
  }
}