using Encounters;
using Encounters.Grid;
using Encounters.Obstacles;
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
      var collectableObject = SceneTerrain.GetTileOccupant(context.TargetedTile);
      if (collectableObject == null) {
        return false;
      }
      var collectable = collectableObject.GetComponentInParent<EncounterCollectable>();
      if (collectable == null) {
        return false;
      }

      return true;
    }
    
    
    protected override void Execute(AbilityExecutionContext context) {
      var collectableObject = SceneTerrain.GetTileOccupant(context.TargetedTile);
      if (collectableObject == null) {
        return;
      }
      var collectable = collectableObject.GetComponentInParent<EncounterCollectable>();
      if (collectable == null) {
        return;
      }
      
      collectable.Collect();
      encounterEvents.abilityExecutionEnd.Raise();
    }
  }
}