using System.Collections;
using Common.Animation;
using Encounters;
using Encounters.Grid;
using Encounters.Obstacles;
using Terrain;
using UnityEngine;

namespace Units.Abilities {
  [CreateAssetMenu(menuName = "Units/Abilities/PlaceObjectAbility")]
  public class PlaceObjectAbility : RangedAbility {
    [SerializeField] private GameObject placedObjectPrefab;
    
    public override void ShowIndicator(
        EncounterActor actor,
        Vector3Int source,
        GameObject hoveredObject,
        Vector3Int hoveredTile,
        GridIndicators indicators) {
      indicators.TargetingIndicator.Clear();
      if (!CanTarget(actor, source, hoveredObject, hoveredTile)) {
        return;
      }
      
      indicators.TargetingIndicator.TargetTiles(hoveredTile);
    }

    public override bool CouldExecute(AbilityExecutionContext context) {
      return CanAfford(context.Actor) &&
          CanTarget(context.Actor, context.Source, context.TargetedObject, context.TargetedTile);
    }

    private bool CanTarget(
        EncounterActor actor,
        Vector3Int source,
        GameObject targetObject,
        Vector3Int targetTile) {
      if (!range.IsInRange(actor, source, targetTile)) {
        return false;
      }

      if (placedObjectPrefab.GetComponent<PlacedObject>().ClaimsTile) {
        if (targetObject.TryGetComponent<IPlacedOnGrid>(out var placedOnGrid) && placedOnGrid.ClaimsTile) {
          return false;
        }
      }

      return SceneTerrain.TryGetTile(targetTile, out _);
    }
    
    protected override IEnumerator Execute(AbilityExecutionContext context, AbilityExecutionCompleteCallback callback) {
      context.Actor.FaceTowards(context.TargetedTile);
      context.Actor.PlayOneOffAnimation(AnimationNames.Attack);
      PlaySound();
      yield return new WaitForSeconds(impactAnimationDelaySec);
      var placedObject = Instantiate(placedObjectPrefab).GetComponent<PlacedObject>();
      placedObject.Init(context.Actor, context.TargetedTile);
      callback();
    }
  }
}