using Common;
using Encounters.AI;
using Optional;
using UnityEngine;

namespace Encounters.Enemies {
  public class SpiritUnitController : EnemyUnitController {
    public Option<Vector3Int> Target { get; set; }

    public override AiActionPlan GetActionPlan(ActionEvaluationContext context) {
      if (EncounterState.GetResourceAmount(exhaustibleResources.mp) == 0
          || !Target.TryGet(out var target)) {
        return new AiActionPlan(this);
      }
      
      // Don't use the terrain for pathfinding, because spirits don't actually care about
      // collisions.

      // TODO IMMEDIATE: update to pre-plan and telegraph actions, then take them.
      return new AiActionPlan(this);
    }
  }
}