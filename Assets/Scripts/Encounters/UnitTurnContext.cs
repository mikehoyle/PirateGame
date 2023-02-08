using System.Collections.Generic;
using CameraControl;
using Controls;
using HUD.Encounter;
using Units;

namespace Encounters {
  /// <summary>
  /// Encapsulates the global context to be passed to a unit when it is that unit's turn.
  /// </summary>
  public class UnitTurnContext {
    public delegate void OnTurnEnded();
    
    public GameControls.TurnBasedEncounterActions Controls { get; set; }
    public MovementPathIndicator TargetingDisplay { get; set; }
    public ActionMenuController ActionMenu { get; set; }
    public CameraCursorMover Camera { get; set; }
    public IEnumerable<UnitEncounterManager> UnitsInEncounter { get; set; }
    public int CurrentTurnIndex { get; set; }
    public OnTurnEnded OnTurnEndedCallback { get; set; }
  }
}