using Optional;
using Units.Abilities;
using UnityEngine;

namespace Encounters.AI {
  /// <summary>
  /// Currently, an action plan simply encompasses a move first, then a skill second. both optional.
  /// </summary>
  public class AiActionPlan {
    public class AiAction {
      public UnitAbility Ability { get; set; }
      public UnitAbility.AbilityExecutionContext Context { get; set; }
    }

    public AiActionPlan(EncounterActor actor) {
      Actor = actor;
      MoveDestination = actor.Position;
    }

    public EncounterActor Actor { get; }
    public Vector3Int MoveDestination { get; set; }
    public Option<AiAction> Action { get; set; } = Option.None<AiAction>();
  }
}