using Encounters;
using Encounters.Grid;
using Encounters.Obstacles;
using Optional;
using Terrain;
using UnityEngine;

namespace Units.Abilities {
  // Default unit ability for collecting resources on the map.
  [CreateAssetMenu(menuName = "Units/Abilities/CollectAbility")]
  public class CollectAbility : UnitAbility {
    
    public override void OnSelected(EncounterActor actor, GridIndicators indicators) {
      indicators.RangeIndicator.DisplayTargetingRange(actor.Position, 1, 1);
    }
    
    public override bool CouldExecute(AbilityExecutionContext context) {
      return TryGetCollectable(context.TargetedObject).HasValue;
    }

    protected override void Execute(AbilityExecutionContext context) {
      TryGetCollectable(context.TargetedObject).MatchSome(collectable => {
        collectable.Collect();
      });
      
      encounterEvents.abilityExecutionEnd.Raise();
    }

    private Option<EncounterCollectable> TryGetCollectable(GameObject gameObject) {
      if (gameObject == null) {
        return Option.None<EncounterCollectable>();
      }

      if (gameObject.TryGetComponent<EncounterCollectable>(out var collectable)) {
        return Option.Some<EncounterCollectable>(collectable);
      }
      
      return Option.None<EncounterCollectable>();
    }
  }
}