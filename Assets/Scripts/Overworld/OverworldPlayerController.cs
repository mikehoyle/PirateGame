using System;
using CameraControl;
using Controls;
using HUD.MainMenu;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Overworld {
  public class OverworldPlayerController : MonoBehaviour, GameControls.IOverworldActions {
    [SerializeField] private float speed;
    
    private GameControls _controls;
    private CameraController _camera;
    private Vector3 _currentVelocity;
    private MainMenuController _gameMenu;

    private void Awake() {
      _camera = Camera.main.GetComponent<CameraController>();
    }
    
    private void OnEnable() {
      _controls ??= new GameControls();
      _controls.Overworld.SetCallbacks(this);
      _controls.Overworld.Enable();
    }

    private void OnDisable() {
      _controls.Overworld.Disable();
    }

    private void Update() {
      transform.localPosition += _currentVelocity * Time.deltaTime;
      _camera.SetFocusPoint(transform.position);
    }

    public void OnMove(InputAction.CallbackContext context) {
      _currentVelocity = context.ReadValue<Vector2>() * speed;
    }
    
    public void OnInteract(InputAction.CallbackContext context) {
      // TODO
    }
  }
}