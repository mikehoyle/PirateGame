using Common;
using Common.Grid;
using Controls;
using Optional;
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
    private Option<Rect> _bounds; 

    private void Awake() {
      _camera = CameraController.Get();
      _bounds = Option.None<Rect>();
    }
    
    private void OnEnable() {
      GameInput.Controls.CameraCursorMovement.SetCallbacks(this);
    }

    private void OnDisable() {
      GameInput.Controls.CameraCursorMovement.RemoveCallbacks(this);
    }

    private void Update() {
      var destination = _cameraCursor + (_cameraCursorVelocity * Time.deltaTime);
      _bounds.Match(
          bounds => {
            var destinationOnGrid = GridUtils.WorldToCell(destination);
            if (bounds.Contains(destinationOnGrid)) {
              _cameraCursor = destination;
            }
          },
          () => _cameraCursor = destination);
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

    public void SetGridBounds(RectInt gridBounds) {
      _bounds = Option.Some(new Rect(
          gridBounds.x,
          gridBounds.y,
          // +1 because coordinates are for cell base, and we want to go to cell edges.
          gridBounds.width + 1f,
          gridBounds.height + 1f));
    }
    
    public void OnMoveCamera(InputAction.CallbackContext context) {
      var normalizedInputVector = context.ReadValue<Vector2>();
      _cameraCursorVelocity = normalizedInputVector * cameraMoveSpeed;
    }
  }
}