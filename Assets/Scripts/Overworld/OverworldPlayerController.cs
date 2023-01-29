using System;
using CameraControl;
using Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Overworld {
  public class OverworldPlayerController : MonoBehaviour, GameControls.IOverworldActions {
    [SerializeField] private float speed;
    
    private GameControls _controls;
    private CameraController _camera;
    private Vector3 _currentVelocity;
    private GameObject _gameMenu;

    private void Awake() {
      _controls = new GameControls();
      _controls.Overworld.SetCallbacks(this);
      _controls.Overworld.Enable();
      _camera = Camera.main.GetComponent<CameraController>();
      _gameMenu = GameObject.FindWithTag(Tags.GameMenu);
      _gameMenu.SetActive(false);
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
    
    public void OnToggleMenu(InputAction.CallbackContext context) {
      _gameMenu.SetActive(!_gameMenu.activeInHierarchy);
    }
  }
}