using System;
using System.Collections;
using System.Collections.Generic;
using Common.Animation;
using Common.Grid;
using StaticConfig.Units;
using Terrain;
using Units;
using UnityEngine;

namespace Encounters.Enemies {
  public class SpiritMover : MonoBehaviour {
    [SerializeField] private ExhaustibleResources exhaustibleResources;
    [SerializeField] private int damageOnCollision = 2;
    [SerializeField] private float speedUnitsPerSec;
    
    private SpiritUnitController _spirit;

    private void Awake() {
      _spirit = GetComponent<SpiritUnitController>();
    }
    
    public IEnumerator ExecuteMovement(List<FacingDirection> plannedPath) {
      if (plannedPath.Count == 0) {
        yield break;
      }

      var currentIndex = 0;
      while (currentIndex < plannedPath.Count) {
        _spirit.FacingDirection = plannedPath[currentIndex];
        var targetPosition = _spirit.Position + (Vector3Int)plannedPath[currentIndex].ToUnitVector();

        if (SceneTerrain.TryGetComponentAtTile<EncounterActor>(targetPosition, out var actor)) {
          actor.ExpendResource(exhaustibleResources.hp, damageOnCollision);
          yield break;
        }

        var worldPositionStart = GridUtils.CellCenterWorldStatic(_spirit.Position);
        var worldPositionEnd = GridUtils.CellCenterWorldStatic(targetPosition);

        var progressToNextNode = 0f;
        while (progressToNextNode < 1) {
          var position = Vector3.Lerp(worldPositionStart, worldPositionEnd, progressToNextNode);
          position.z = 1f;
          transform.position = position;
          progressToNextNode += (speedUnitsPerSec * Time.deltaTime);
          yield return null;
        }
        
        _spirit.Position = targetPosition;
        worldPositionEnd.z = 1f;
        transform.position = worldPositionEnd;

        if (SceneTerrain.TryGetComponentAtTile<Bones>(targetPosition, out var bones)) {
          // TODO(P0): Actually collect the bones and revive the unit
          Debug.Log("Would revive unit here.");
          yield return StartCoroutine(_spirit.Dissipate());
          yield break;
        }
        currentIndex++;
      }
    }
  }
}