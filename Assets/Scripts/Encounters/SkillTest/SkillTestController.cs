using System.Collections;
using Controls;
using Units.Abilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Encounters.SkillTest {
  /// <summary>
  /// TODO(P1): This is jank AF, proof of concept only.
  /// </summary>
  public class SkillTestController : MonoBehaviour, GameControls.ISkillTestActions {
    private const float Duration = 1.5f;
    private SkillTestArm _arm;
    private GameControls _controls;

    private float _elapsedTime;
    private UnitAbility.AbilityEffectivenessCallback _callback;
    
    private void Awake() {
      _arm = GetComponentInChildren<SkillTestArm>();
      enabled = false;
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.SkillTest.SetCallbacks(this);
      }
      
      _controls.SkillTest.Enable();
    }

    private void OnDisable() {
      _controls?.SkillTest.Disable();
    }

    private void Update() {
      _elapsedTime += Time.deltaTime;

      if (_elapsedTime >= Duration) {
        Stop();
      }
    }

    public void Run(UnitAbility.AbilityEffectivenessCallback callback) {
      enabled = true;
      _elapsedTime = 0;
      _callback = callback;
      _arm.Run(Duration);
    }
    
    public void OnInteract(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      Stop();
    }

    private void Stop() {
      var result = _arm.Stop();
      _callback.Invoke(result);
      Destroy(gameObject);
    }
  }
}