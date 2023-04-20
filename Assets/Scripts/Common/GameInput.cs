using System.Collections.Generic;
using Controls;

namespace Common {
  public static class GameInput {
    private static HashSet<object> _inputBlockers = new();
    
    private static GameControls _controls;
    public static GameControls Controls {
      get {
        if (_controls == null) {
          _controls = new GameControls();
          EnableControls();
        }
        return _controls;
      }
    }

    public static void AddInputBlocker(object blocker) {
      _inputBlockers.Add(blocker);
      DisableControls();
    }

    public static void RemoveInputBlocker(object blocker) {
      _inputBlockers.Remove(blocker);
      if (_inputBlockers.Count == 0) {
        EnableControls();
      }
    }

    /// <summary>
    /// Disables just the in-game controls that should be paused for eg. a cinematic.
    /// </summary>
    private static void DisableControls() {
      Controls.ShipManagement.Disable();
      Controls.ShipBuilder.Disable();
      Controls.ShipPlacement.Disable();
      Controls.Overworld.Disable();
      Controls.TurnBasedEncounter.Disable();
      Controls.SkillTest.Disable();
      Controls.HoverTile.Disable();
      Controls.CameraCursorMovement.Disable();
    }
    
    private static void EnableControls() {
      Controls.ShipManagement.Enable();
      Controls.ShipBuilder.Enable();
      Controls.ShipPlacement.Enable();
      Controls.Overworld.Enable();
      Controls.TurnBasedEncounter.Enable();
      Controls.SkillTest.Enable();
      Controls.HoverTile.Enable();
      Controls.CameraCursorMovement.Enable();
    }
  }
}